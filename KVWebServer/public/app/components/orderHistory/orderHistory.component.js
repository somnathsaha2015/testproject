"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var app_service_1 = require('../../services/app.service');
var OrderHistory = (function () {
    function OrderHistory(appService) {
        var _this = this;
        this.appService = appService;
        this.subscription = appService.filterOn('get:order:headers')
            .subscribe(function (d) {
            _this.orderHeaders = JSON.parse(d.data).Table;
            console.log(d);
        });
    }
    ;
    OrderHistory.prototype.showDetails = function (id) {
        console.log(id);
    };
    ;
    OrderHistory.prototype.ngOnInit = function () {
        var token = this.appService.getToken();
        this.appService.httpGet('get:order:headers', { token: token });
    };
    ;
    OrderHistory.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    ;
    OrderHistory = __decorate([
        core_1.Component({
            templateUrl: 'app/components/orderHistory/orderHistory.component.html'
        }), 
        __metadata('design:paramtypes', [app_service_1.AppService])
    ], OrderHistory);
    return OrderHistory;
}());
exports.OrderHistory = OrderHistory;
//# sourceMappingURL=orderHistory.component.js.map