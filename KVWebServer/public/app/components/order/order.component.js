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
var Order = (function () {
    function Order(appService) {
        var _this = this;
        this.appService = appService;
        this.staticTexts = {
            introText: this.appService.getMessage('mess:order:intro:text'),
            holidayGift: this.appService.getMessage('mess:order:holiday:gift'),
            minimumRequest: this.appService.getMessage('mess:order:minimum:request'),
            bottomNotes: this.appService.getMessage('mess:order:bottom:notes')
        };
        //this.staticTexts.introText = appService.getMessage('mess:order:intro:text');
        this.currentOfferSubscription = appService.filterOn('get:current:offer')
            .subscribe(function (d) {
            _this.orders = JSON.parse(d.data).Table;
            console.log(d);
        });
        this.saveOrderSubscription = appService.filterOn('post:save:order')
            .subscribe(function (d) {
            console.log(d);
        });
    }
    ;
    Order.prototype.request = function () {
        //console.log(this.orders);
        var finalOrder = this.orders.map(function (value, i) {
            return ({ OfferId: value.Id, OrderQty: value.OrderQty, WishList: value.WishList });
        });
        var token = this.appService.getToken();
        this.appService.httpPost('post:save:order', { token: token, order: finalOrder });
    };
    ;
    Order.prototype.ngOnInit = function () {
        var token = this.appService.getToken();
        this.appService.httpGet('get:current:offer', { token: token });
    };
    ;
    Order.prototype.ngOnDestroy = function () {
        this.currentOfferSubscription.unsubscribe();
        this.saveOrderSubscription.unsubscribe();
    };
    Order = __decorate([
        core_1.Component({
            templateUrl: 'app/components/order/order.component.html'
        }), 
        __metadata('design:paramtypes', [app_service_1.AppService])
    ], Order);
    return Order;
}());
exports.Order = Order;
//# sourceMappingURL=order.component.js.map