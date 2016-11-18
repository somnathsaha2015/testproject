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
var Login = (function () {
    function Login(appService, router) {
        var _this = this;
        this.appService = appService;
        this.router = router;
        this.subscription = appService.filterOn('post:authenticate')
            .subscribe(function (d) {
            console.log(d);
            if (d.data.error) {
                console.log(d.data.error.status);
                appService.resetCredential();
            }
            else {
                console.log('token:' + d.data.token);
                _this.appService.setCredential(_this.email, d.data.token);
                _this.router.navigate(['order']);
            }
        });
    }
    ;
    Login.prototype.authenticate = function (pwd) {
        var base64Encoded = this.appService.encodeBase64(this.email + ':' + md5_1.md5(pwd));
        console.log('md5:' + md5_1.md5(pwd));
        console.log(base64Encoded);
        this.appService.httpPost('post:authenticate', { auth: base64Encoded });
    };
    ;
    Login.prototype.logout = function () {
        this.appService.resetCredential();
        this.router.navigate(['/login']);
    };
    Login.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
    };
    Login = __decorate([
        core_1.Component({
            templateUrl: 'app/components/login/login.component.html'
        }), 
        __metadata('design:paramtypes', [app_service_1.AppService, router_1.Router])
    ], Login);
    return Login;
}());
exports.Login = Login;
//# sourceMappingURL=login.component.js.map