import { Injectable, Inject } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ClientService extends BaseApiService {

    constructor(private http: HttpClient) {

        super(http, `Client`);
    }
}