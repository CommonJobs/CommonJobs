$(document).ready(function () {
    ko.applyBindings(new CommonFood.MenuViewModel({
        title: "Menú Primaveral",
        firstWeek: 1,
        firstDay: 4,
        weeks: 4,
        options: [
            "Común", 
            "Light", 
            "Vegetariano"
        ],
        startDate: new Date("2012-09-21"),
        endDate: new Date("2012-12-20")
    }));
});
