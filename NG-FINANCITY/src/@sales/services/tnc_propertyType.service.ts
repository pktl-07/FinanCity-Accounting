import { Injectable, Inject } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class TNC_PropertyTypeService extends BaseApiService  {

    constructor(public http: HttpClient) {

        super(http, `TNC_PropertyType`);
    }
}