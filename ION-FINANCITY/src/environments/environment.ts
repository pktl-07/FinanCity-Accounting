// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { BehaviorSubject } from "rxjs";
import { IFCConfig } from "src/@common/utils";

export const environment: IFCConfig = {
  auth: {
    api: 'https://localhost:44301',
    email: null,
    isAuthenticate: new BehaviorSubject(false),
    licenseKey: '123123',
    profileName: new BehaviorSubject(null),
    loginType: 'password',
  },
  general: {
    currSK: '2016121916540307550',
    currCode: 'SGD',
    profilePhoto: new BehaviorSubject(null),
    chattingCounter: new BehaviorSubject(0),
    mortageSalesCounter: new BehaviorSubject(0),
    referralSalesCounter: new BehaviorSubject(0),
    version: new BehaviorSubject(0),
    leadFollowUpCounter: new BehaviorSubject(0)
  },
  defaultSalesTax: {
    taxSK: '201307051139513774165001',
    taxCode: 'SR',
    taxPercent: 7,
  },
  defaultPurchaseTax: {
    taxSK: '201307051129191477965001',
    taxCode: 'TX7',
    taxPercent: 7,
  },
  defaultPurchaseZeroTax: {
    taxSK: '1',
    taxCode: 'T0',
    taxPercent: 0,
  },
  dbReady: new BehaviorSubject(false),
  production: false
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
