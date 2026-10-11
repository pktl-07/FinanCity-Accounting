import { FinanCityConfig } from "src/@common/Infrastructure/common-interface";

export const environment: FinanCityConfig = {
  production: true,
  auth: {
    api: 'https://financityapi.peas.sg',
    email: localStorage.getItem('email'),
    isAuthenticate: localStorage.getItem('isAuthenticate') == 'true' ? true : false,
    licenseKey: null,
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
