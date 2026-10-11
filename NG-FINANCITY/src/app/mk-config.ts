import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { FinanCityConfig } from 'src/@common/Infrastructure/common-interface';

@Injectable({ providedIn: 'root' })
export class GlobalConfigurationSubject {

    public constructor() {

        this.FinanCityConfig = new BehaviorSubject<FinanCityConfig>(environment);
        this.ChattingCounter = new BehaviorSubject(0);
        this.MortgageSalesCounter = new BehaviorSubject(0);
        this.ReferralSalesCounter = new BehaviorSubject(0);
    }
    public FinanCityConfig: BehaviorSubject<FinanCityConfig>;
    public ChattingCounter: BehaviorSubject<number>;
    public MortgageSalesCounter: BehaviorSubject<number>;
    public ReferralSalesCounter: BehaviorSubject<number>;
}

// export const MKConfig: IMKConfig = {
//     auth: {
//         //api: 'https://localhost:44301',
//         api: 'https://financityapi.peas.sg',
//         //api: 'https://demofinancityapi.peas.sg',
//         email: localStorage.getItem('email'),
//         isAuthenticate: localStorage.getItem('isAuthenticate') == 'true' ? true : false,
//         licenseKey: '123123',
//         profileName: localStorage.getItem('profileName'),
//     }, general: {
//         currSK: '2016121916540307550',
//         currCode: 'SGD',
//     }, defaultSalesTax: {
//         taxSK: '201307051139513774165001',
//         taxCode: 'SR',
//         taxPercent: 7,
//     }, defaultPurchaseTax: {
//         taxSK: '201307051129191477965001',
//         taxCode: 'TX7',
//         taxPercent: 7,
//     }, defaultPurchaseZeroTax: {
//         taxSK: '1',
//         taxCode: 'T0',
//         taxPercent: 0,
//     }
// }