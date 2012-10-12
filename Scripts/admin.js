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
        startDate: new Date(),
        endDate: new Date("2012-12-20")
    }));
});
