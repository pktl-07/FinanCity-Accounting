import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PageUtilityService {

    public taxSubject = new BehaviorSubject<{ key: string, code: string, percent: number }>({
        code: 'N/A',
        key: null,
        percent: 0
    });

    public marketingItemSubject = new BehaviorSubject<any[]>([]);
    public grossCommissionAmountSubject = new BehaviorSubject<number>(0);
    public clientSubject = new BehaviorSubject<any[]>([]);
    public internalSalespersonSubject = new BehaviorSubject<any[]>([]);
    public externalSalespersonSubject = new BehaviorSubject<any[]>([]);
    public PTOSKSubject = new BehaviorSubject<string>(null);
}