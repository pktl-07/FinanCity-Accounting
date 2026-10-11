import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable()
export class PageUtilityService {

    constructor() {
    }

    promotionItemExpenseSubject = new BehaviorSubject<[]>([]);
    clientSubject = new BehaviorSubject<[]>([]);
    internalSubject = new BehaviorSubject<[]>([]);
    externalSubject = new BehaviorSubject<[]>([]);
    commissionSource = new BehaviorSubject<any[]>([]);

    taxSubject = new BehaviorSubject<{ Key: string, Value: string, TaxPercent: number }>({
        Key: environment.defaultSalesTax.taxSK,
        Value: environment.defaultSalesTax.taxCode,
        TaxPercent: environment.defaultSalesTax.taxPercent
    });
}