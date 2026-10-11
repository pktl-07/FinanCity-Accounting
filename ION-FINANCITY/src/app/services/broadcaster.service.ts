import { Injectable } from '@angular/core';
import { Subject ,  Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';

export interface BroadcastEvent {
    key: any;
    data?: any;
}

@Injectable({ providedIn: 'root' })
export class BroadcasterService {

    private _eventBus = new Subject<BroadcastEvent>();

    broadcast(key: any, data?: any) {
        this._eventBus.next({ key, data });
    }

    on<T>(key: any): Observable<T> {

        return this._eventBus.pipe(
            filter((event) => event.key === key),
            map(event => <T>event.data));
    }
}