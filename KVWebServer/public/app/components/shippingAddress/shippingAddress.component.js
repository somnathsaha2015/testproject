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
var ShippingAddress = (function () {
    function ShippingAddress(appService) {
        var _this = this;
        this.appService = appService;
        this.isDefault = true;
        this.getSubscription = appService.filterOn("get:shipping:address")
            .subscribe(function (d) {
            _this.addresses = JSON.parse(d.data).Table;
            console.log(d);
        });
        this.postSubscription = appService.filterOn("post:shipping:address")
            .subscribe(function (d) {
            if (d.data.error) {
                console.log(d.data.error);
            }
            else {
                _this.appService.httpGet('get:shipping:address', { token: _this.appService.getToken() });
            }
        });
    }
    ;
    ShippingAddress.prototype.ngOnInit = function () {
        this.appService.httpGet('get:shipping:address', { token: this.appService.getToken() });
    };
    ShippingAddress.prototype.toggleEdit = function (address) {
        var isEdit = address.isEdit;
        this.addresses.map(function (d, i) { return d.isEdit = false; });
        if (isEdit) {
            address.isEdit = false;
        }
        else {
            address.isEdit = true;
        }
        address.isDirty = true;
    };
    ;
    ShippingAddress.prototype.setDefault = function (address) {
        this.addresses.map(function (d, i) { d.isDefault = false; d.isDirty = true; });
        address.isDefault = true;
    };
    ;
    ShippingAddress.prototype.submit = function () {
        var dirtyAddresses = this.addresses.filter(function (v, i) { return v.isDirty; });
        if (dirtyAddresses.length > 0) {
            var token = this.appService.getToken();
            this.appService.httpPost('post:shipping:address', { token: token, addresses: dirtyAddresses });
        }
    };
    ;
    ShippingAddress.prototype.addAddress = function () {
        var address = { address1: '', city: '', zip: '', street: '', isNew: true, isDirty: true };
        this.addresses.unshift(address);
    };
    ;
    ShippingAddress.prototype.removeNew = function (index) {
        this.addresses.splice(index, 1);
    };
    ;
    ShippingAddress.prototype.ngOnDestroy = function () {
        this.getSubscription.unsubscribe();
        this.postSubscription.unsubscribe();
    };
    ;
    ShippingAddress = __decorate([
        core_1.Component({
            templateUrl: 'app/components/shippingAddress/shippingAddress.component.html'
        }), 
        __metadata('design:paramtypes', [app_service_1.AppService])
    ], ShippingAddress);
    return ShippingAddress;
}());
exports.ShippingAddress = ShippingAddress;
//# sourceMappingURL=shippingAddress.component.js.map