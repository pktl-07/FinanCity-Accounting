import { Injectable, OnDestroy } from '@angular/core';
import { PGridIncellEditingConfig } from 'src/@common/Infrastructure/common-interface';
import { BehaviorSubject } from 'rxjs';
import { SubSink } from 'src/@common/service/utility-service';

const cloneData = (data: any[]) => data.map(item => Object.assign({}, item));

@Injectable()
export class GridInCellEditService extends BehaviorSubject<any> implements OnDestroy {

    public data: any[] = [];
    public originalData: any[] = [];
    public createdItems: any[] = [];
    public updatedItems: any[] = [];
    public deletedItems: any[] = [];

    public _config: PGridIncellEditingConfig;

    constructor() {
        super([]);
    }

    private _subsink = new SubSink();
    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    read(config: PGridIncellEditingConfig) {

        this._config = config;
        this._subsink.sink = this._config.fetch
            .subscribe(data => {

                this.data = data;
                if (this.data == null)
                    this.data = [];

                this.originalData = cloneData(this.data);
                this.createdItems = [];
                this.updatedItems = [];
                this.deletedItems = [];

                if (this._config.rowDataBound != undefined) {
                    for (let idx = 0; idx < data.length; idx++) { // notify handler
                        this._config.rowDataBound({ data: this.data[idx], index: idx });
                    }
                }
                super.next(data);
            });
    }

    public itemIndex(item: any, data: any[]): number {

        for (let idx = 0; idx < data.length; idx++) {

            if (data[idx][this._config.key]
                && data[idx][this._config.key] != null
                && data[idx][this._config.key] === item[this._config.key]) {

                return idx;
            }
        }
        return -1;
    }

    public create(item: any): void {

        this.createdItems.push(item);
        this.data.push(item);

        super.next(this.data);
    }

    public update(item: any): void {

        if (!this.isNew(item)) {
            const index = this.itemIndex(item, this.updatedItems);
            if (index !== -1) {
                this.updatedItems.splice(index, 1, item);
            } else {
                this.updatedItems.push(item);
            }
        } else {
            const index = this.createdItems.indexOf(item);
            this.createdItems.splice(index, 1, item);
        }
    }

    remove(item: any, index: number): void {

        this.data.splice(index, 1);

        index = this.itemIndex(item, this.originalData);
        if (index >= 0) {
            this.deletedItems.push(item);
        }

        index = this.itemIndex(item, this.updatedItems);
        if (index >= 0) {
            this.updatedItems.splice(index, 1);
        }

        index = this.itemIndex(item, this.createdItems);
        if (index >= 0) {
            this.deletedItems.push(item);
        } else {
            index = this.createdItems.indexOf(item);
            this.createdItems.splice(index, 1);
        }

        super.next(this.data);
    }

    isNew(item: any): boolean {
        return item[this._config.key] == null;
    }

    hasChanges(): boolean {
        return Boolean(this.deletedItems.length || this.updatedItems.length || this.createdItems.length);
    }

    cancelChanges(): void {
        this.reset();

        this.data = this.originalData;
        this.originalData = cloneData(this.originalData);
        super.next(this.data);
    }

    assignValues(target: any, source: any): void {
        Object.assign(target, source);
    }

    public reset() {

        this.data = [];
        this.deletedItems = [];
        this.updatedItems = [];
        this.createdItems = [];
    }

    public addAt(item: any, index: number) {

        this.data.splice(index, 0, item);
        super.next(this.data);
    }
}
