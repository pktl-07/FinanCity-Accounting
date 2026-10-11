import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class EventsService extends BehaviorSubject<any> {

    constructor() {
        super([]);
    }
}