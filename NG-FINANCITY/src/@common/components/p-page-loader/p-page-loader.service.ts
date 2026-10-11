import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PageLoaderService {

    private _loading: boolean = false;
    get loading(): boolean {
        return this._loading;
    }
    set loading(value: boolean) {
        this._loading = value;
    }

    private _submitting: boolean;
    get submitting(): boolean {
        return this._submitting;
    }
    set submitting(value: boolean) {
        this._submitting = value;
    }

    circularLoading = new BehaviorSubject(false);
}