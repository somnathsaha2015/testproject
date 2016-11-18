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
//import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
var app_service_1 = require('../../services/app.service');
var config_1 = require('../../config');
var ng2_modal_1 = require("ng2-modal");
var ApproveOrder = (function () {
    function ApproveOrder(appService) {
        var _this = this;
        this.appService = appService;
        this.approveHeading = config_1.messages['mess:approve:heading'];
        this.defaultAddress = {};
        // $(document).ready(function () {
        //     console.log("ready!");
        // });
        this.defaultAddrSubscription = appService.filterOn('get:default:shipping:address').subscribe(function (d) {
            if (d.data.error) {
                console.log(d.data.error);
            }
            else {
                _this.defaultAddress = JSON.parse(d.data).Table[0];
            }
        });
        this.allAddrSubscription = appService.filterOn('get:all:shipping:addresses').subscribe(function (d) {
            if (d.data.error) {
                console.log(d.data.error);
            }
            else {
                _this.allAddresses = JSON.parse(d.data).Table;
            }
        });
    }
    ;
    ApproveOrder.prototype.changeDefaultAddr = function () {
        this.appService.httpGet('get:all:shipping:addresses');
        this.myModal.open();
    };
    ;
    ApproveOrder.prototype.setDefault = function (address) {
        this.defaultAddress = address;
        this.myModal.close();
    };
    // @ViewChild('childModal') public childModal: ModalDirective;
    // public showChildModal(): void {
    //     this.childModal.show();
    // }
    // public hideChildModal(): void {
    //     this.childModal.hide();
    // }
    ApproveOrder.prototype.ngOnInit = function () {
        this.appService.httpGet('get:default:shipping:address');
    };
    ApproveOrder.prototype.ngOnDestroy = function () {
        this.defaultAddrSubscription.unsubscribe();
        this.allAddrSubscription.unsubscribe();
    };
    ;
    __decorate([
        core_1.ViewChild('myModal'), 
        __metadata('design:type', ng2_modal_1.Modal)
    ], ApproveOrder.prototype, "myModal", void 0);
    ApproveOrder = __decorate([
        core_1.Component({
            templateUrl: 'app/components/approveOrder/approveOrder.component.html'
        }), 
        __metadata('design:paramtypes', [app_service_1.AppService])
    ], ApproveOrder);
    return ApproveOrder;
}());
exports.ApproveOrder = ApproveOrder;
//# sourceMappingURL=approveOrder.component.js.map