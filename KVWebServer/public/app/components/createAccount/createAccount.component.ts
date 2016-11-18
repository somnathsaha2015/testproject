import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs/subscription';
import { AppService } from '../../services/app.service';
import { md5 } from '../../vendor/md5';
@Component({
    templateUrl: 'app/components/createAccount/createAccount.component.html'
})
export class CreateAccount {
    email: string;
    subscription: Subscription;
    constructor(private appService: AppService, private router: Router) {
        this.subscription = appService.filterOn('post:create:account')
            .subscribe(d => {
                console.log(d);
                if (d.data.error) {
                    console.log(d.data.error.status)
                    appService.resetCredential();
                } else {
                    console.log('success');
                }
            });
    };
    createAccount(pwd, confirmPwd) {
        if (pwd === confirmPwd) {
            let data = { email: this.email, hash: md5(pwd) };
            this.appService.httpPost('post:create:account', data);
        }

    };
    ngOnDestroy() {
        this.subscription.unsubscribe();
    };
}
