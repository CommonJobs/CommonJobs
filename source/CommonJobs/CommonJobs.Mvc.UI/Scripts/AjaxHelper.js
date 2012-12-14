var AjaxHelper;
(function (AjaxHelper) {
    var BunchProcessor = (function () {
        function BunchProcessor(getData, process, calculatePendingCount, final) {
            this.getData = getData;
            this.process = process;
            this.calculatePendingCount = calculatePendingCount;
            this.final = final;
        }
        BunchProcessor.prototype.getPendingCount = function () {
            return this.pendingCount;
        };
        BunchProcessor.prototype.getTaken = function () {
            return this.taken;
        };
        BunchProcessor.prototype.getPercentage = function () {
            return this.getTaken() / (this.getTaken() + this.getPendingCount());
        };
        BunchProcessor.prototype.updateState = function (data, take, skip) {
            this.taken = take + skip;
            this.pendingCount = this.calculatePendingCount ? this.calculatePendingCount(data, take, skip) : (data && (typeof data.length == 'undefined' || data.length) ? Infinity : 0);
            if(this.pendingCount < 0) {
                this.taken += this.pendingCount;
                this.pendingCount = 0;
            }
        };
        BunchProcessor.prototype.run = function (take, skip) {
            var _this = this;
            skip = skip || 0;
            this.getData(take, skip, function (data) {
                _this.updateState(data, take, skip);
                var pending = _this.getPendingCount() > 0;
                pending && _this.run(take, skip + take);
                _this.process && _this.process(data, take, skip);
                pending || _this.final && _this.final(data, take, skip + take);
            });
        };
        return BunchProcessor;
    })();
    AjaxHelper.BunchProcessor = BunchProcessor;    
})(AjaxHelper || (AjaxHelper = {}));

