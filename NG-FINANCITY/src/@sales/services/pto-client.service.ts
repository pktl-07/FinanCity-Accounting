import { Injectable, Inject } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class PTOClientService extends BaseApiService {

    constructor(http: HttpClient) {

        super(http, `PTOClient`);
    }
}