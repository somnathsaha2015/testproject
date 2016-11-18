import { Http, Response, Headers, RequestOptionsArgs } from '@angular/http';
import { Injectable } from '@angular/core';
//import { CanActivate } from '@angular/router';
import { Subject } from 'rxjs/Subject';
//import { AsyncSubject } from 'rxjs/AsyncSubject';
import { Observable } from 'rxjs/Rx';

import {
    CanActivate,
    Router,
    ActivatedRouteSnapshot,
    RouterStateSnapshot
} from '@angular/router';
import 'rxjs/add/operator/map'; //this is how operator is imported
import 'rxjs/add/operator/filter';
//import * as _ from 'lodash';
import { urlHash, messages } from '../config';

@Injectable()
export class AppService {
    subject: Subject<any>;
    channel: any;
    globalHash: {} = {};

    constructor(private http: Http) {
        this.subject = new Subject();
        this.channel = {};
    };

    getMessage(messageKey) {
        return (messages[messageKey]);
    }

    setCredential(email, token) {
        let credential = { email: email, token: token };
        localStorage.setItem('credential', JSON.stringify(credential));
    };
    getCredential(): any {
        let credentialString = localStorage.getItem('credential');
        let credential = null;
        if (credentialString) {
            credential = JSON.parse(credentialString);
        }
        return (credential);
    };
    getToken(): string {
        let token = null;
        let credential = this.getCredential();
        if (credential) {
            token = credential.token;
        }
        return (token);
    }
    resetCredential() {
        localStorage.removeItem('credential');
    };

    httpPost(id: string, body?: any) {
        let url = urlHash[id];
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('x-access-token', this.getToken());
        body.token = this.getToken();
        this.http.post(url, body,{headers:headers})
            .map(response => response.json())
            .subscribe(d =>
                this.subject.next({
                    id: id, data: d,body:body
                }), err =>
                this.subject.next({
                    id: id,
                    data: { error: err }
                }));
    };
    httpGet(id: string, body?: {}) {
        let url = urlHash[id];        
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('x-access-token', this.getToken());
        this.http.get(url, { headers: headers })
            .map(response => response.json())
            .subscribe(d =>
                this.subject.next({
                    id: id, data: d
                }), err =>
                this.subject.next({
                    id: id,
                    data: { error: err }
                }));
    };
    httpDelete(id: string, body?: any) {
        let url = urlHash[id];
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('x-access-token', this.getToken());
        this.http.delete(url, { headers: headers, body: { id: body.id } })
            .map(response => response.json())
            .subscribe(d =>
                this.subject.next({
                    id: id, data: d, index: body.index
                }), err =>
                this.subject.next({
                    id: id,
                    data: { error: err }
                }));
    };
    //application wide events
    emit(id: string, options?: any) {
        this.subject.next({
            id: id, data: options
        });
    }

    filterOn(id: string): Observable<any> {
        return (this.subject.filter(d => (d.id === id)));
    }

    reply(key: string, value: any) {
        this.channel[key] = value;
    };
    request(key: string, payload?: any): any {
        let ret: any = undefined;
        if (payload) {
            ret = this.channel[key](payload);
        } else {
            ret = this.channel[key];
        }
        return (ret);
    };

    encodeBase64(inputString) {
        let Base64 = { _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=", encode: function (e) { var t = ""; var n, r, i, s, o, u, a; var f = 0; e = Base64._utf8_encode(e); while (f < e.length) { n = e.charCodeAt(f++); r = e.charCodeAt(f++); i = e.charCodeAt(f++); s = n >> 2; o = (n & 3) << 4 | r >> 4; u = (r & 15) << 2 | i >> 6; a = i & 63; if (isNaN(r)) { u = a = 64 } else if (isNaN(i)) { a = 64 } t = t + this._keyStr.charAt(s) + this._keyStr.charAt(o) + this._keyStr.charAt(u) + this._keyStr.charAt(a) } return t }, decode: function (e) { var t = ""; var n, r, i; var s, o, u, a; var f = 0; e = e.replace(/[^A-Za-z0-9\+\/\=]/g, ""); while (f < e.length) { s = this._keyStr.indexOf(e.charAt(f++)); o = this._keyStr.indexOf(e.charAt(f++)); u = this._keyStr.indexOf(e.charAt(f++)); a = this._keyStr.indexOf(e.charAt(f++)); n = s << 2 | o >> 4; r = (o & 15) << 4 | u >> 2; i = (u & 3) << 6 | a; t = t + String.fromCharCode(n); if (u != 64) { t = t + String.fromCharCode(r) } if (a != 64) { t = t + String.fromCharCode(i) } } t = Base64._utf8_decode(t); return t }, _utf8_encode: function (e) { e = e.replace(/\r\n/g, "\n"); var t = ""; for (var n = 0; n < e.length; n++) { var r = e.charCodeAt(n); if (r < 128) { t += String.fromCharCode(r) } else if (r > 127 && r < 2048) { t += String.fromCharCode(r >> 6 | 192); t += String.fromCharCode(r & 63 | 128) } else { t += String.fromCharCode(r >> 12 | 224); t += String.fromCharCode(r >> 6 & 63 | 128); t += String.fromCharCode(r & 63 | 128) } } return t }, _utf8_decode: function (e) { var t = ""; var c1: number, c2: number, c3: any; var n = 0; var r = c1 = c2 = 0; while (n < e.length) { r = e.charCodeAt(n); if (r < 128) { t += String.fromCharCode(r); n++ } else if (r > 191 && r < 224) { c2 = e.charCodeAt(n + 1); t += String.fromCharCode((r & 31) << 6 | c2 & 63); n += 2 } else { c2 = e.charCodeAt(n + 1); c3 = e.charCodeAt(n + 2); t += String.fromCharCode((r & 15) << 12 | (c2 & 63) << 6 | c3 & 63); n += 3 } } return t } }
        return (Base64.encode(inputString));
    }
};

@Injectable()
export class LoginGuard implements CanActivate {
    constructor(private appService: AppService, private http: Http, private router: Router)
    { }

    canActivate(
        // Not using but worth knowing about
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        let ret: any = false;
        let credential = this.appService.getCredential();
        if (credential) {
            let token = credential.token;
            if (token) {
                ret = this.isLoggedIn(token);
            } else {
                this.router.navigate(['/login']);
            }
        }
        return (ret);
    };

    isLoggedIn(token): Observable<boolean> | boolean {
        //let router: Router = this.router;
        let obs;
        try {
            //let options = new RequestOptions({body:{token:token}});
            let url = urlHash['post:validate:token'];
            obs = this.http.post(url, { token: token })
                .map(result => result.json());
        } catch (err) {
            obs = Observable.of(false);
        }

        return obs
            .map(success => {
                // navigate to login page
                if (!success) {
                    this.router.navigate(['/login']);
                }
                return success;
            });
    }
}

