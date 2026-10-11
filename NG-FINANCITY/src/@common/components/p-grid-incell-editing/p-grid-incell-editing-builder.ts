import { PGridIncellEditingComponent } from './p-grid-incell-editing.component';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { Injectable } from '@angular/core';
import { PGridInCellColumn } from 'src/@common/Infrastructure/common-interface';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class GridIncellEditingBuilder {

    public _component: PGridIncellEditingComponent;

    constructor() {
    }

    Intialize(component: PGridIncellEditingComponent) {

        this._component = component;
        return this;
    }

    key(key: string) {

        this._component.config.key = key;
        return this;
    }

    fetch(fetch: BaseApiService | BehaviorSubject<any>) {

        this._component.config.fetch = fetch;
        return this;
    }

    height(height: number) {

        this._component.config.height = height;
        return this;
    }

    rowDataBount(rowDataBound?: (e: { data: any, index?: number }) => void) {

        this._component.config.rowDataBound = rowDataBound;
        return this;
    }

    columns(columns: Array<PGridInCellColumn>) {

        this._component.config.columns = columns;
        return this;
    }
}
