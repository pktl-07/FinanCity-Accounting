import { BehaviorSubject } from 'rxjs';
import { PGridViewConfig } from 'src/@common/Infrastructure/common-interface';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Injectable, OnDestroy } from '@angular/core';
import { SubSink } from 'src/@common/service/utility-service';

@Injectable()
export class GridViewService extends BehaviorSubject<GridDataResult> implements OnDestroy {

    config: PGridViewConfig;
    private _subsink = new SubSink();

    constructor() {
        super({ data: [], total: 0 });
    }

    read(config: PGridViewConfig) {

        this.config = config;
        this._subsink.sink = this.config.fetch
            .subscribe((data) => {
                if (data && data.data != null) {
                    if (this.config.rowDataBound) {
                        for (let idx = 0; idx < data.data.length; idx++) { // notify handler
                            this.config.rowDataBound({ data: data.data[idx], index: idx });
                        }
                    }
                    super.next({ data: data.data, total: data.total });
                }
                else if (data != null) {
                    if (this.config.rowDataBound) {
                        for (let idx = 0; idx < data.length; idx++) { // notify handler
                            this.config.rowDataBound({ data: data[idx], index: idx });
                        }
                    }
                    super.next({ data: data, total: data.length });
                }
            });
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }
}