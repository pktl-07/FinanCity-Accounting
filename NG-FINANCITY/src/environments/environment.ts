import { FinanCityConfig } from "src/@common/Infrastructure/common-interface";

export const environment: FinanCityConfig = {
  production: false,
  auth: {
    api: 'https://localhost:44301',
    email: localStorage.getItem('email'),
    isAuthenticate: localStorage.getItem('isAuthenticate') == 'true' ? true : false,
    licenseKey: '123123',
    profileName: localStorage.getItem('profileName'),
  }, general: {
    currSK: '2016121916540307550',
    currCode: 'SGD',
  }, defaultSalesTax: {
    taxSK: '201307051139513774165001',
    taxCode: 'SR',
    taxPercent: 7,
  }, defaultPurchaseTax: {
    taxSK: '201307051129191477965001',
    taxCode: 'TX7',
    taxPercent: 7,
  }, defaultPurchaseZeroTax: {
    taxSK: '1',
    taxCode: 'T0',
    taxPercent: 0,
  }
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
