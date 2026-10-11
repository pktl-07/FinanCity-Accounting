import { Injectable, OnDestroy } from '@angular/core';
import { Network } from '@ionic-native/network/ngx'
import { BehaviorSubject, Observable } from 'rxjs';
import { Platform } from '@ionic/angular';
import { SubSink } from 'src/@common/utils';

export enum ConnectionStatus {
    Online,
    Offline
}

@Injectable({ providedIn: 'root' })
export class FCNetworkService implements OnDestroy {

    private _subsink = new SubSink();
    private status: BehaviorSubject<ConnectionStatus> = new BehaviorSubject(ConnectionStatus.Offline);

    constructor(private _network: Network,
        private _platform: Platform) {

        this._platform.ready().then(() => {
            this.initializeNetworkEvents();
            let status = this._network.type !== 'none' ? ConnectionStatus.Online : ConnectionStatus.Offline;
            this.status.next(status);
        })
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    public onNetworkChange(): Observable<ConnectionStatus> {
        return this.status.asObservable();
    }

    public getCurrentNetworkStatus(): ConnectionStatus {
        return this.status.getValue();
    }

    private async updateNetworkStatus(status: ConnectionStatus) {
        this.status.next(status);
    }

    private initializeNetworkEvents() {

        this._subsink.sink = this._network.onDisconnect()
            .subscribe(() => {
                if (this.status.getValue() === ConnectionStatus.Online) {
                    this.updateNetworkStatus(ConnectionStatus.Offline);
                }
            })

        this._subsink.sink = this._network.onConnect()
            .subscribe(() => {
                if (this.status.getValue() === ConnectionStatus.Offline) {
                    this.updateNetworkStatus(ConnectionStatus.Online);
                }
            })
    }
}