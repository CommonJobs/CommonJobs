declare class Twix {
    constructor (start, end, allDay);
    countDays: () => number;
    sameDay: () => bool;
    sameYear: () => bool;
    daysIn: (minHours?: number) => { 
        next: () => moment.Moment;
        hasNext: () => bool;
    };
}