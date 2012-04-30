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

        public Dictionary<string, MigrationDescriptor> GetMigrationStatus()
        {
            var validMigrations = ListValidMigrations();
            var installedMigrations = ListInstalledMigrations();
            var result =
                validMigrations.Keys.Union(installedMigrations.Keys)
                .Select(x => GetDescriptorWithUpdatedStatus(x, validMigrations, installedMigrations))
                .ToDictionary(x => x.Id);
            return result;
        }

        private static MigrationDescriptor GetDescriptorWithUpdatedStatus(string key, Dictionary<string, MigrationDescriptor> validMigrations, Dictionary<string, MigrationDescriptor> installedMigrations)
        {
            MigrationDescriptor auxValid = null;
            MigrationDescriptor auxInstalled = null;
            if (validMigrations.TryGetValue(key, out auxValid))
            {
                auxValid.Status = installedMigrations.TryGetValue(key, out auxInstalled) ? auxInstalled.Status : MigrationStatus.NotInstalled;
            }
            else
            {
                auxInstalled = installedMigrations[key];
                auxInstalled.Status = MigrationStatus.InstalledObsolete;
            }
            return auxValid ?? auxInstalled;
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
                .Where(x => x.attribute != null)
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
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    descriptor.Status = MigrationStatus.Installing;
                    session.Store(descriptor);
                    session.SaveChanges();
                }
                IMigration migration;
                if (TryCreateMigrationInstance(descriptor, out migration))
                {
                    migration.Up();
                }
                using (var session = DocumentStore.OpenSession())
                {
                    descriptor.Status = MigrationStatus.Installed;
                    session.Store(descriptor);
                    session.SaveChanges();
                }
            }
            catch
            {
                //Por ahora no estoy haciendo nada con esto, si no se pudo aplicar la migración se va a ver en el resumen
            }
        }

        public void Down(MigrationDescriptor descriptor)
        {
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    descriptor.Status = MigrationStatus.Uninstalling;
                    session.Store(descriptor);
                    session.SaveChanges();
                }
                IMigration migration;
                if (TryCreateMigrationInstance(descriptor, out migration))
                {
                    migration.Down();
                }
                using (var session = DocumentStore.OpenSession())
                {
                    var stored = session.Load<MigrationDescriptor>(descriptor.Id);
                    if (stored != null)
                    {
                        session.Delete(stored);
                        session.SaveChanges();
                    }
                }
            }
            catch
            {
                //Por ahora no estoy haciendo nada con esto, si no se pudo deshacer la migración se va a ver en el resumen
            }
        }

        public void UpAll()
        {
            var descriptors = GetMigrationStatus();
            var toInstall = descriptors
                .Values
                .OrderBy(x => x.Id) //¿Esto es necesario?
                .Where(x => x.Status != MigrationStatus.Installed && x.Status != MigrationStatus.InstalledObsolete);
            foreach (var descriptor in toInstall)
                Up(descriptor);
        }

        public void RunActions(IEnumerable<MigrationAction> actions)
        {
            var descriptors = GetMigrationStatus();
            var itemsToRun = actions
                .Where(x => x.Action != MigrationActionType.None)
                .OrderBy(x => x.Id)
                .Select(x => new { descriptor = descriptors[x.Id], action = x.Action == MigrationActionType.Down ? (Action<MigrationDescriptor>)Down : Up });
            foreach (var itemToRun in itemsToRun)
                itemToRun.action(itemToRun.descriptor);
        }
    }
}
