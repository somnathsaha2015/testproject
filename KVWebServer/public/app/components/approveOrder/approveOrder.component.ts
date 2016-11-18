import { Component, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs/subscription';
//import { ModalDirective } from 'ng2-bootstrap/ng2-bootstrap';
import { AppService } from '../../services/app.service';
import { messages } from '../../config';
import {ModalModule,Modal} from "ng2-modal";

@Component({
    templateUrl: 'app/components/approveOrder/approveOrder.component.html'
})
export class ApproveOrder {
    defaultAddrSubscription: Subscription;
    allAddrSubscription:Subscription;
    approveHeading: string = messages['mess:approve:heading'];
    defaultAddress: {} = {};
    allAddresses:[any];
    constructor(private appService: AppService) {
        // $(document).ready(function () {
        //     console.log("ready!");
        // });
        this.defaultAddrSubscription = appService.filterOn('get:default:shipping:address').subscribe(d => {
            if (d.data.error) {
                console.log(d.data.error);
            } else {
                this.defaultAddress = JSON.parse(d.data).Table[0]
            }
        });
        this.allAddrSubscription = appService.filterOn('get:all:shipping:addresses').subscribe(d => {
            if (d.data.error) {
                console.log(d.data.error);
            } else {
                this.allAddresses = JSON.parse(d.data).Table;
            }
        });
    };
    @ViewChild('myModal') myModal:Modal;
    changeDefaultAddr() {
        this.appService.httpGet('get:all:shipping:addresses');
        this.myModal.open();
    };
    setDefault(address){
        this.defaultAddress=address;
        this.myModal.close();
    }
    // @ViewChild('childModal') public childModal: ModalDirective;

    // public showChildModal(): void {
    //     this.childModal.show();
    // }

    // public hideChildModal(): void {
    //     this.childModal.hide();
    // }
    ngOnInit() {
        this.appService.httpGet('get:default:shipping:address');
    }
    ngOnDestroy() {
        this.defaultAddrSubscription.unsubscribe();
        this.allAddrSubscription.unsubscribe();
    };
}