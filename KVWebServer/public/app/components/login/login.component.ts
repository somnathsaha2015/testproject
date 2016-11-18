import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs/subscription';
import { AppService } from '../../services/app.service';
import {viewBoxConfig} from '../../config';
import { md5 } from '../../vendor/md5';

@Component({
  templateUrl: 'app/components/login/login.component.html'
})
export class Login {
  email: string;
  subscription: Subscription;
  constructor(private appService: AppService, private router: Router) {
    this.subscription = appService.filterOn('post:authenticate')
      .subscribe(d => {
        console.log(d);
        if(d.data.error){
          console.log(d.data.error.status)
          appService.resetCredential();          
        } else {
          console.log('token:' + d.data.token);
          this.appService.setCredential(this.email, d.data.token);
          this.router.navigate(['order']);
        }
      });
  };
  authenticate(pwd) {
    let base64Encoded = this.appService.encodeBase64(this.email + ':' + md5(pwd));
    console.log('md5:' + md5(pwd));
    console.log(base64Encoded);
    this.appService.httpPost('post:authenticate', { auth: base64Encoded });
  };
  logout(){
    this.appService.resetCredential();  
    this.router.navigate(['/login']);
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
