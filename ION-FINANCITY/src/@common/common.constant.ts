export const GenderSource = [
    { value: 0, text: 'MALE' },
    { value: 1, text: 'FEMALE' }];

export const InformationSource = [{ value: 'COMPANY', text: 'COMPANY' },
{ value: 'PERSONAL', text: 'PERSONAL' },
{ value: 'FACEBOOK', text: 'FACEBOOK' },
{ value: 'GOOGLE', text: 'GOOGLE' },
{ value: 'REFERRAL', text: 'REFERRAL' },
{ value: 'OTHERS', text: 'OTHERS' }];

export const ActiveSource = [
    { value: true, text: 'YES' },
    { value: false, text: 'NO' }]

export const SalutationSource = [
    { value: 'MR', text: 'MR' },
    { value: 'MRS', text: 'MRS' },
    { value: 'MS', text: 'MS' },
    { value: 'MDM', text: 'MDM' },
    { value: 'DR', text: 'DR' }];

export const MartialStatusSource = [
    { value: 0, text: 'SINGLE' },
    { value: 1, text: 'MARRIED' }];

export const SaleActivity = [{ value: 'PROSPECTING', text: 'PROSPECTING' },
{ value: 'PRESENTATION', text: 'PRESENTATION' },
{ value: 'SALES FOLLOW-UP', text: 'SALES FOLLOW-UP' },
{ value: 'SALES CLOSURE', text: 'SALES CLOSURE' }];

export const VendorType = [
    { value: 15, text: 'REA' },
    { value: 16, text: 'PROPERTY DEVELOPER' },
    { value: 17, text: 'LAW FIRM' },
    { value: 18, text: 'BANK' },
    { value: 100, text: 'REFERRAL' },
    { value: 19, text: 'OTHERS' }];

export const cloneData = (data: any[]) => data.map(item => Object.assign({}, item));