import { Component } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Router, NavigationEnd, Event } from '@angular/router';
import { Subscription } from 'rxjs/subscription';
import { AppService } from '../services/app.service';
import { viewBoxConfig } from '../config';
//import * as Rx from 'rxjs/rx';

@Component({
  selector: 'my-app',
  templateUrl: 'app/components/app.component.html'
})

export class AppComponent {
  subscription: Subscription;
  viewBox: {} = viewBoxConfig['/login'];

  constructor(private appService: AppService, private router: Router) {

    //Catching up Router event by name 'NavigationEnd'

    // router.events.filter((e: any) => {
    router.events.filter((e: Event, t: number) => {
      return (e.constructor.name === 'NavigationEnd');
    }).subscribe((event: any) => {
      let url = event.urlAfterRedirects.split('?')[0];
      this.viewBox = viewBoxConfig[url];
    });

  };
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
