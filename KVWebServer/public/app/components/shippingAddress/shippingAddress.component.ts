import { Component } from '@angular/core';
import { Subscription } from 'rxjs/subscription';
import { AppService } from '../../services/app.service';

@Component({
    templateUrl: 'app/components/shippingAddress/shippingAddress.component.html'
})
export class ShippingAddress {
    getSubscription: Subscription;
    postSubscription: Subscription;
    addresses: [{}];
    isDefault: boolean = true;
    constructor(private appService: AppService) {
        this.getSubscription = appService.filterOn("get:shipping:address")
            .subscribe(d => {
                this.addresses = JSON.parse(d.data).Table;
                console.log(d);
            });
        this.postSubscription = appService.filterOn("post:shipping:address")
            .subscribe(d => {
                if (d.data.error) {
                    console.log(d.data.error);
                } else {
                    this.appService.httpGet('get:shipping:address', { token: this.appService.getToken() });
                }                
            });
    };
    ngOnInit() {
        this.appService.httpGet('get:shipping:address', { token: this.appService.getToken() });
    }
    toggleEdit(address) {
        let isEdit = address.isEdit;
        this.addresses.map((d: any, i) => d.isEdit = false);
        if (isEdit) {
            address.isEdit = false;
        } else {
            address.isEdit = true;
        }
        address.isDirty = true;
    };
    setDefault(address) {
        this.addresses.map((d: any, i) => { d.isDefault = false; d.isDirty = true; });
        address.isDefault = true;
    };
    submit() {
        let dirtyAddresses = this.addresses.filter((v: any, i) => v.isDirty);
        if (dirtyAddresses.length > 0) {
            let token = this.appService.getToken();
            this.appService.httpPost('post:shipping:address', { token: token, addresses: dirtyAddresses });
        }
    };
    addAddress() {
        let address = {address1:'',city:'',zip:'',street:'',isNew:true,isDirty:true};
        this.addresses.unshift(address);
    };
    removeNew(index) {
      this.addresses.splice(index,1);  
    };
    ngOnDestroy() {
        this.getSubscription.unsubscribe();
        this.postSubscription.unsubscribe();
    };
}