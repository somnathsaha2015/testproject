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
var router_1 = require('@angular/router');
var app_service_1 = require('../../services/app.service');
var md5_1 = require('../../vendor/md5');
var ForgotPassword = (function () {
    function ForgotPassword(appService, router) {
        var _this = this;
        this.appService = appService;
        this.router = router;
        this.subscription = appService.filterOn('post:forgot:password')
            .subscribe(function (d) {
            if (d.data.error) {
                console.log(d.data.error.status);
            }
            else {
                console.log('Success');
                _this.router.navigate(['/login']);
            }
        });
    }
    ;
    ForgotPassword.prototype.sendMail = function () {
        var base64Encoded = this.appService.encodeBase64(this.email);
        this.appService.httpPost('post:forgot:password', { auth: base64Encoded });
    };
    ForgotPassword.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    ForgotPassword = __decorate([
        core_1.Component({
            templateUrl: 'app/components/managePassword/forgotPassword.component.html'
        }), 
        __metadata('design:paramtypes', [app_service_1.AppService, router_1.Router])
    ], ForgotPassword);
    return ForgotPassword;
}());
exports.ForgotPassword = ForgotPassword;
//send password component
var SendPassword = (function () {
    function SendPassword(appService, router) {
        var _this = this;
        this.appService = appService;
        this.router = router;
        this.subscription = appService.filterOn('post:send:password')
            .subscribe(function (d) {
            if (d.data.error) {
                console.log(d.data.error.status);
            }
            else {
                console.log('Success');
                _this.router.navigate(['/login']);
            }
        });
    }
    ;
    SendPassword.prototype.sendPassword = function () {
        var code = window.location.search.split('=')[1];
        console.log(code);
        this.appService.httpPost('post:send:password', { auth: code });
    };
    SendPassword.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    SendPassword = __decorate([
        core_1.Component({
            template: "\n  <button (click)=\"sendPassword()\">Send Password</button>\n  "
        }), 
        __metadata('design:paramtypes', [app_service_1.AppService, router_1.Router])
    ], SendPassword);
    return SendPassword;
}());
exports.SendPassword = SendPassword;
//change password component
var ChangePassword = (function () {
    function ChangePassword(appService, router) {
        var _this = this;
        this.appService = appService;
        this.router = router;
        this.subscription = appService.filterOn('post:change:password')
            .subscribe(function (d) {
            if (d.data.error) {
                console.log(d.data.error.status);
            }
            else {
                _this.appService.resetCredential();
                console.log('Success');
            }
            _this.router.navigate(['/login']);
        });
    }
    ;
    ChangePassword.prototype.changePassword = function (oldPwd, newPwd1, newPwd2) {
        var credential = this.appService.getCredential();
        if (credential) {
            var email = credential.email;
            if (email) {
                if (newPwd1 === newPwd2) {
                    var base64Encoded = this.appService.encodeBase64(email + ':' + md5_1.md5(oldPwd) + ':' + md5_1.md5(newPwd1));
                    console.log(base64Encoded);
                    this.appService.httpPost('post:change:password', { auth: base64Encoded, token: credential.token });
                }
            }
            else {
                this.router.navigate(['/login']);
            }
        }
    };
    ChangePassword.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    ChangePassword = __decorate([
        core_1.Component({
            templateUrl: 'app/components/managePassword/changePassword.component.html'
        }), 
        __metadata('design:paramtypes', [app_service_1.AppService, router_1.Router])
    ], ChangePassword);
    return ChangePassword;
}());
exports.ChangePassword = ChangePassword;
//# sourceMappingURL=managePassword.component.js.map