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
var platform_browser_1 = require('@angular/platform-browser');
var http_1 = require('@angular/http');
var forms_1 = require('@angular/forms');
var app_component_1 = require('./components/app.component');
var login_component_1 = require('./components/login/login.component');
var createAccount_component_1 = require('./components/createAccount/createAccount.component');
var managePassword_component_1 = require('./components/managePassword/managePassword.component');
var order_component_1 = require('./components/order/order.component');
//import {ChildComponent} from './childComponent';
var app_service_1 = require('./services/app.service');
//import { LoginGuard } from './services/app.loginGuard';
var app_routes_1 = require('./components/routes/app.routes');
var profile_component_1 = require('./components/profile/profile.component');
var approveOrder_component_1 = require('./components/approveOrder/approveOrder.component');
var receipt_component_1 = require('./components/receipt/receipt.component');
var orderHistory_component_1 = require('./components/orderHistory/orderHistory.component');
var shippingAddress_component_1 = require('./components/shippingAddress/shippingAddress.component');
var paymentMethod_component_1 = require('./components/paymentMethod/paymentMethod.component');
//import { ModalModule } from 'ng2-bootstrap/ng2-bootstrap';
var ng2_modal_1 = require('ng2-modal');
//import { jquery } from 'jquery';
//import {Route1, Route2, Home} from './app.routes.components';
//import {ComponentStub1} from './componentStub1';
var AppModule = (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        core_1.NgModule({
            imports: [platform_browser_1.BrowserModule, http_1.HttpModule, app_routes_1.Routing, forms_1.FormsModule,
                ng2_modal_1.ModalModule
            ],
            declarations: [app_component_1.AppComponent, login_component_1.Login, order_component_1.Order, managePassword_component_1.ForgotPassword,
                managePassword_component_1.SendPassword, managePassword_component_1.ChangePassword, createAccount_component_1.CreateAccount,
                profile_component_1.Profile, approveOrder_component_1.ApproveOrder, receipt_component_1.Receipt, orderHistory_component_1.OrderHistory, shippingAddress_component_1.ShippingAddress, paymentMethod_component_1.PaymentMethod],
            providers: [app_service_1.AppService, app_service_1.LoginGuard],
            bootstrap: [app_component_1.AppComponent]
        }), 
        __metadata('design:paramtypes', [])
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map