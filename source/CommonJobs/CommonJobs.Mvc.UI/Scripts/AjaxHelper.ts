module AjaxHelper {
    export class BunchProcessor {

        private pendingCount: number;
        private taken: number;

        constructor (
            private getData: (take: number, skip: number, callback: (data) => void ) => void,
            private process?: (data: any, take?: number, skip?: number) => void,
            private calculatePendingCount?: (data: any, take?: number, skip?: number) => number,
            private final?: (data?: any, take?: number, skip?: number) => void) {
        }

        getPendingCount() {
            return this.pendingCount;
        }

        getTaken() {
            return this.taken;
        }

        getPercentage() {
            return this.getTaken() / (this.getTaken() + this.getPendingCount());
        }

        private updateState(data: any, take: number, skip: number) {
            this.taken = take + skip;
            this.pendingCount = this.calculatePendingCount ? this.calculatePendingCount(data, take, skip)
                : (data && (typeof data.length == 'undefined' || data.length) ? Infinity : 0);
            if (this.pendingCount < 0) {
                this.taken += this.pendingCount;
                this.pendingCount = 0;
            }  
        }

        run(take: number, skip?: number) : void {
            skip = skip || 0;
            //console.log("getData start " + skip);
            this.getData(take, skip, (data) => {
                //console.log("getData finish " + skip);
                this.updateState(data, take, skip);
                //console.log(this.getPercentage() + " " + this.getPendingCount() + " " + this.getTaken() );
                var pending = this.getPendingCount() > 0;
                //setTimeout(() => {
                    pending && this.run(take * 2, skip + take);
                //}, 0);
                //setTimeout(() => {
                    //console.log("process start " + skip);
                    this.process && this.process(data, take, skip);
                    //console.log("process finish " + skip);
                    pending || this.final && this.final(data, take, skip + take);
                //}, 0);
            });
        }
    }
}
