using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using System.Reflection;

namespace CommonJobs.Raven.Migrations
{
    public class Migrator 
    {
        public IDocumentStore DocumentStore { get; set; }
        public Assembly MigrationsAssembly { get; set; }

        public Migrator(IDocumentStore documentStore, Assembly migrationsAssembly)
        {
            DocumentStore = documentStore;
            MigrationsAssembly = migrationsAssembly;
        }

        public List<MigrationStatus> GetMigrationStatus()
        {
            var validMigrations = ListValidMigrations();
            var installedMigrations = ListInstalledMigrations();
            MigrationDescriptor aux;
            var result =
                validMigrations.Keys.Union(installedMigrations.Keys)
                .OrderBy(x => x)
                .Select(x => validMigrations.TryGetValue(x, out aux) 
                    ? new MigrationStatus() { MigrationDescriptor = aux, Obsolete = false, Installed = installedMigrations.ContainsKey(x) }
                    : new MigrationStatus() { MigrationDescriptor = installedMigrations[x], Obsolete = true, Installed = true })
                .ToList();
            return result;
        }

        private Dictionary<string, MigrationDescriptor> ListValidMigrations()
        {
            var iMigration = typeof(IMigration);
            var migrationAttribute = typeof(MigrationAttribute);
            var types = MigrationsAssembly.GetTypes();
            var result = types
                .Where(x => iMigration.IsAssignableFrom(x))
                .Select(x => new { migrationType = x, attribute = x.GetCustomAttributes(migrationAttribute, false).OfType<MigrationAttribute>().FirstOrDefault() })
                .Where(x => x.migrationType != null)
                .Select(x => new MigrationDescriptor() { Id = x.attribute.Id, Description = x.attribute.Description, MigrationTypeFullName = x.migrationType.FullName })
                .ToDictionary(x => x.Id);
            return result;
        }

        private Dictionary<string, MigrationDescriptor> ListInstalledMigrations()
        {
            using (var session = DocumentStore.OpenSession())
            {
                return session.Query<MigrationDescriptor>().Customize(x => x.WaitForNonStaleResultsAsOfLastWrite()).ToDictionary(x => x.Id);
            }
        }

        private bool TryCreateMigrationInstance(MigrationDescriptor descriptor, out IMigration migration)
        {
            migration = null;
            var result =
                descriptor.MigrationTypeFullName != null
                && (migration = Activator.CreateInstance(MigrationsAssembly.FullName, descriptor.MigrationTypeFullName).Unwrap() as IMigration) != null;

            if (result)
            {
                migration.DocumentStore = DocumentStore;
            }

            return result;
        }

        public void Up(MigrationDescriptor descriptor)
        {
            IMigration migration;
            if (TryCreateMigrationInstance(descriptor, out migration))
            {
                migration.Up();
            }
            using (var session = DocumentStore.OpenSession())
            {
                session.Store(descriptor);
                session.SaveChanges();
            }
        }

        public void Down(MigrationDescriptor descriptor)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var stored = session.Load<MigrationDescriptor>(descriptor.Id);
                if (stored != null)
                {
                    session.Delete(stored);
                    session.SaveChanges();
                }
            }
            IMigration migration;
            if (TryCreateMigrationInstance(descriptor, out migration))
            {
                migration.Down();
            }
        }

        public void UpAll()
        {
            var status = GetMigrationStatus();
            foreach (var migrationStatus in status)
            {
                if (migrationStatus.Obsolete)
                    Down(migrationStatus.MigrationDescriptor);
                else if (!migrationStatus.Installed)
                    Up(migrationStatus.MigrationDescriptor);
            }
        }
    }
}
