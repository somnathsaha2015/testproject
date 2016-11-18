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
var http_1 = require('@angular/http');
var core_1 = require('@angular/core');
//import { CanActivate } from '@angular/router';
var Subject_1 = require('rxjs/Subject');
//import { AsyncSubject } from 'rxjs/AsyncSubject';
var Rx_1 = require('rxjs/Rx');
var router_1 = require('@angular/router');
require('rxjs/add/operator/map'); //this is how operator is imported
require('rxjs/add/operator/filter');
//import * as _ from 'lodash';
var config_1 = require('../config');
var AppService = (function () {
    function AppService(http) {
        this.http = http;
        this.globalHash = {};
        this.subject = new Subject_1.Subject();
        this.channel = {};
    }
    ;
    AppService.prototype.getMessage = function (messageKey) {
        return (config_1.messages[messageKey]);
    };
    AppService.prototype.setCredential = function (email, token) {
        var credential = { email: email, token: token };
        localStorage.setItem('credential', JSON.stringify(credential));
    };
    ;
    AppService.prototype.getCredential = function () {
        var credentialString = localStorage.getItem('credential');
        var credential = null;
        if (credentialString) {
            credential = JSON.parse(credentialString);
        }
        return (credential);
    };
    ;
    AppService.prototype.getToken = function () {
        var token = null;
        var credential = this.getCredential();
        if (credential) {
            token = credential.token;
        }
        return (token);
    };
    AppService.prototype.resetCredential = function () {
        localStorage.removeItem('credential');
    };
    ;
    AppService.prototype.httpPost = function (id, body) {
        var _this = this;
        var url = config_1.urlHash[id];
        var headers = new http_1.Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('x-access-token', this.getToken());
        body.token = this.getToken();
        this.http.post(url, body, { headers: headers })
            .map(function (response) { return response.json(); })
            .subscribe(function (d) {
            return _this.subject.next({
                id: id, data: d, body: body
            });
        }, function (err) {
            return _this.subject.next({
                id: id,
                data: { error: err }
            });
        });
    };
    ;
    AppService.prototype.httpGet = function (id, body) {
        var _this = this;
        var url = config_1.urlHash[id];
        var headers = new http_1.Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('x-access-token', this.getToken());
        this.http.get(url, { headers: headers })
            .map(function (response) { return response.json(); })
            .subscribe(function (d) {
            return _this.subject.next({
                id: id, data: d
            });
        }, function (err) {
            return _this.subject.next({
                id: id,
                data: { error: err }
            });
        });
    };
    ;
    AppService.prototype.httpDelete = function (id, body) {
        var _this = this;
        var url = config_1.urlHash[id];
        var headers = new http_1.Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('x-access-token', this.getToken());
        this.http.delete(url, { headers: headers, body: { id: body.id } })
            .map(function (response) { return response.json(); })
            .subscribe(function (d) {
            return _this.subject.next({
                id: id, data: d, index: body.index
            });
        }, function (err) {
            return _this.subject.next({
                id: id,
                data: { error: err }
            });
        });
    };
    ;
    //application wide events
    AppService.prototype.emit = function (id, options) {
        this.subject.next({
            id: id, data: options
        });
    };
    AppService.prototype.filterOn = function (id) {
        return (this.subject.filter(function (d) { return (d.id === id); }));
    };
    AppService.prototype.reply = function (key, value) {
        this.channel[key] = value;
    };
    ;
    AppService.prototype.request = function (key, payload) {
        var ret = undefined;
        if (payload) {
            ret = this.channel[key](payload);
        }
        else {
            ret = this.channel[key];
        }
        return (ret);
    };
    ;
    AppService.prototype.encodeBase64 = function (inputString) {
        var Base64 = { _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=", encode: function (e) { var t = ""; var n, r, i, s, o, u, a; var f = 0; e = Base64._utf8_encode(e); while (f < e.length) {
                n = e.charCodeAt(f++);
                r = e.charCodeAt(f++);
                i = e.charCodeAt(f++);
                s = n >> 2;
                o = (n & 3) << 4 | r >> 4;
                u = (r & 15) << 2 | i >> 6;
                a = i & 63;
                if (isNaN(r)) {
                    u = a = 64;
                }
                else if (isNaN(i)) {
                    a = 64;
                }
                t = t + this._keyStr.charAt(s) + this._keyStr.charAt(o) + this._keyStr.charAt(u) + this._keyStr.charAt(a);
            } return t; }, decode: function (e) { var t = ""; var n, r, i; var s, o, u, a; var f = 0; e = e.replace(/[^A-Za-z0-9\+\/\=]/g, ""); while (f < e.length) {
                s = this._keyStr.indexOf(e.charAt(f++));
                o = this._keyStr.indexOf(e.charAt(f++));
                u = this._keyStr.indexOf(e.charAt(f++));
                a = this._keyStr.indexOf(e.charAt(f++));
                n = s << 2 | o >> 4;
                r = (o & 15) << 4 | u >> 2;
                i = (u & 3) << 6 | a;
                t = t + String.fromCharCode(n);
                if (u != 64) {
                    t = t + String.fromCharCode(r);
                }
                if (a != 64) {
                    t = t + String.fromCharCode(i);
                }
            } t = Base64._utf8_decode(t); return t; }, _utf8_encode: function (e) { e = e.replace(/\r\n/g, "\n"); var t = ""; for (var n = 0; n < e.length; n++) {
                var r = e.charCodeAt(n);
                if (r < 128) {
                    t += String.fromCharCode(r);
                }
                else if (r > 127 && r < 2048) {
                    t += String.fromCharCode(r >> 6 | 192);
                    t += String.fromCharCode(r & 63 | 128);
                }
                else {
                    t += String.fromCharCode(r >> 12 | 224);
                    t += String.fromCharCode(r >> 6 & 63 | 128);
                    t += String.fromCharCode(r & 63 | 128);
                }
            } return t; }, _utf8_decode: function (e) { var t = ""; var c1, c2, c3; var n = 0; var r = c1 = c2 = 0; while (n < e.length) {
                r = e.charCodeAt(n);
                if (r < 128) {
                    t += String.fromCharCode(r);
                    n++;
                }
                else if (r > 191 && r < 224) {
                    c2 = e.charCodeAt(n + 1);
                    t += String.fromCharCode((r & 31) << 6 | c2 & 63);
                    n += 2;
                }
                else {
                    c2 = e.charCodeAt(n + 1);
                    c3 = e.charCodeAt(n + 2);
                    t += String.fromCharCode((r & 15) << 12 | (c2 & 63) << 6 | c3 & 63);
                    n += 3;
                }
            } return t; } };
        return (Base64.encode(inputString));
    };
    AppService = __decorate([
        core_1.Injectable(), 
        __metadata('design:paramtypes', [http_1.Http])
    ], AppService);
    return AppService;
}());
exports.AppService = AppService;
;
var LoginGuard = (function () {
    function LoginGuard(appService, http, router) {
        this.appService = appService;
        this.http = http;
        this.router = router;
    }
    LoginGuard.prototype.canActivate = function (
        // Not using but worth knowing about
        next, state) {
        var ret = false;
        var credential = this.appService.getCredential();
        if (credential) {
            var token = credential.token;
            if (token) {
                ret = this.isLoggedIn(token);
            }
            else {
                this.router.navigate(['/login']);
            }
        }
        return (ret);
    };
    ;
    LoginGuard.prototype.isLoggedIn = function (token) {
        var _this = this;
        //let router: Router = this.router;
        var obs;
        try {
            //let options = new RequestOptions({body:{token:token}});
            var url = config_1.urlHash['post:validate:token'];
            obs = this.http.post(url, { token: token })
                .map(function (result) { return result.json(); });
        }
        catch (err) {
            obs = Rx_1.Observable.of(false);
        }
        return obs
            .map(function (success) {
            // navigate to login page
            if (!success) {
                _this.router.navigate(['/login']);
            }
            return success;
        });
    };
    LoginGuard = __decorate([
        core_1.Injectable(), 
        __metadata('design:paramtypes', [AppService, http_1.Http, router_1.Router])
    ], LoginGuard);
    return LoginGuard;
}());
exports.LoginGuard = LoginGuard;
//# sourceMappingURL=app.service.js.map