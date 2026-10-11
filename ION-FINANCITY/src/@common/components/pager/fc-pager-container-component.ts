import { Component, Input, OnInit, ContentChild, OnDestroy, TemplateRef, AfterViewInit, AfterContentInit, ViewChild } from '@angular/core';
import { FCCardDirective } from './directives/fc-card.directive';
import { FCListDirective } from './directives/fc-list.directive';
import { FCCardHeaderDirective } from './directives/fc-card-header.directive';
import { FCCardItemDirective } from './directives/fc-card-item.directive';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { DataResult, State } from '@progress/kendo-data-query';
import { BehaviorSubject } from 'rxjs';
import { FCFilterToolbarComponent } from '../toolbar/fc-filter-toolbar.component';
import { FilterConnectorService } from '../validator/fc.filter-connector.service';
import { FCEmptyDataTemplate } from './directives/fc-empty-data.directive';
import { logoPicture, SubSink } from 'src/@common/utils';
import { FCCustomDataDirective } from './directives/fc-custom-data-directive';
import { IonicSafeString, IonInfiniteScroll } from '@ionic/angular';

/**
 * @example
 * <fc-pager-container [data]="data" filterable="true">
 *   <fc-card>
 *      <fc-card-header [text]="ProductID" viewDetail="true"></fc-card-header>
 *       <fc-card-item field="ProductName" filterable="true" title="Product Name"></fc-pager-item>
 *       <fc-card-item field="UnitPrice" filterable="true" title="UnitPrice"></fc-pager-item>
 *   </fc-card>
 * </fc-pager-container>
 */
@Component({
  selector: 'fc-pager-container',
  template: `
  <ion-list>
    <ng-container *ngIf="(view | async)?.length == 0">
      <ng-container *ngTemplateOutlet="emptyItemTemplate"></ng-container>
    </ng-container>

    <ng-template #emptyItemTemplate>
      <ion-grid>
        <ion-row class="ion-justify-content-center ion-align-items-center">
          <ion-text class="text-muted">
            No Records Found.
          </ion-text>    
        </ion-row>
        <ion-row class="ion-justify-content-center ion-align-items-center"  *ngIf="emptyDataTemplate?.templateRef">
            <ng-template [ngIf]="emptyDataTemplate?.templateRef"
              [ngTemplateOutlet]="emptyDataTemplate?.templateRef"
              [ngTemplateOutletContext]="emptyDataTemplateContext">
            </ng-template>
        </ion-row>
      </ion-grid>
    </ng-template>
  
    <ng-container *ngIf="cardCompnentTemplate">
      <ion-card *ngFor="let dataItem of view | async" (click)="onClickDetail(dataItem)">
        <ion-item>
          <ion-avatar *ngIf="cardHeaderTemplate?.icon">  
            <ion-img *ngIf="cardHeaderTemplate.icon" [src]="getPicture(cardHeaderTemplate?.icon)"></ion-img>
          </ion-avatar>  
          <ion-label *ngIf="cardHeaderTemplate?.field">&nbsp;<span style="font-size: .9rem">{{ dataItem[cardHeaderTemplate?.field] }}</span></ion-label>
          <ng-template [ngIf]="cardHeaderTemplate?.headerTemplate?.templateRef"
            [ngTemplateOutlet]="cardHeaderTemplate?.headerTemplate?.templateRef"
            [ngTemplateOutletContext]="headerDataTemplateContext(dataItem)">
          </ng-template>
        </ion-item>
        <ion-card-content>
          <div *ngIf="cardItemTemplate?.field">
            {{ nullDefault(dataItem[cardItemTemplate?.field]) }}
          </div>
          <ng-template [ngIf]="cardItemTemplate?.itemTemplate?.templateRef"
            [ngTemplateOutlet]="cardItemTemplate?.itemTemplate?.templateRef"
            [ngTemplateOutletContext]="itemDataTemplateContext(dataItem)">
          </ng-template>
        </ion-card-content>
      </ion-card>
    </ng-container>

    <ng-container *ngIf="customDataDirective?.templateRef && (view | async)?.length > 0">
      <div *ngFor="let dataItem of view | async;let index=index">
        <ng-template [ngIf]="!customDataDirective?.templateRef" 
          [ngTemplateOutlet]="customDataDirective?.templateRef"
          [ngTemplateOutletContext]="customDataTemplateContext(dataItem,index)">
        </ng-template>
      </div> 
    </ng-container>

    <ion-infinite-scroll #infiniteScroll threshold="10px" (ionInfinite)="onInfiniteScroll($event)">
      <ion-infinite-scroll-content
        loadingSpinner="bubbles"
        loadingText="Loading more data...">
      </ion-infinite-scroll-content>
    </ion-infinite-scroll>
  </ion-list>
  `,
  styles: [`
    .center-div{
      height: 400px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
  `]
})
export class FCPagerContainerComponent implements OnInit, OnDestroy {

  view = new BehaviorSubject<any[]>([]);

  private _subsink = new SubSink();
  @Input() buttonText = "View";

  readonly _take = 14; // page size.
  public _skip: number = 0;
  public _currentPage: number = 1; // current page.
  public _totalRecods: number = 0; // number of records fetched from server.

  @Input() recordInfoSubject = new BehaviorSubject(""); // information to be shown on the separate footer.
  @Input() data: BaseApiService | BehaviorSubject<any>;
  @Input() method: string = "getPager";
  @Input() toolbar: FCFilterToolbarComponent;
  @Input() connector: FilterConnectorService;
  @Input() filterValue: any;

  @ContentChild(FCCustomDataDirective, { static: true }) customDataDirective: FCCustomDataDirective;
  @ContentChild(FCListDirective, { static: true }) listComponentTemplate: FCListDirective;
  @ContentChild(FCCardDirective, { static: true }) cardCompnentTemplate: FCCardDirective;
  @ContentChild(FCCardHeaderDirective, { static: true }) cardHeaderTemplate: FCCardHeaderDirective;
  @ContentChild(FCCardItemDirective, { static: true }) cardItemTemplate: FCCardItemDirective;
  @ContentChild(FCEmptyDataTemplate, { static: true }) emptyDataTemplate: FCEmptyDataTemplate;
  @ViewChild(IonInfiniteScroll, { static: true }) infiniteScroll: IonInfiniteScroll;

  private _timeout = [];
  //private _schrollEvent: any; // for pager schroll event to set enable = true and false value.
  private _emptyDataTemplateContext: any = {};

  public get emptyDataTemplateContext(): any {
    const context = this._emptyDataTemplateContext;
    context.$explicit = this.connector.filterValue;
    context.filterText = this.connector.filterValue;

    return this._emptyDataTemplateContext;
  }

  private _headerDataTemplateContext: any = {};
  headerDataTemplateContext(item: any) {

    const context = this._headerDataTemplateContext;
    context.dataItem = item;
    return context;
  }

  private _itemDataTemplateContext: any = {};
  itemDataTemplateContext(item: any) {

    const context = this._itemDataTemplateContext;
    context.dataItem = item;
    return context;
  }

  public get emptyDataTemplateRef(): TemplateRef<any> {
    return this.emptyDataTemplate ? this.emptyDataTemplate.templateRef : undefined;
  }

  getPicture(key: string) {

    return logoPicture[key];
  }

  nullDefault(value: any) {

    if (typeof value == 'undefined' || value == null || value == '')
      return 'N/A'

    return value;
  }

  ngOnInit(): void {

    this._subsink.sink = this.data
      .subscribe((d: DataResult) => {
        if (d && d != null) {
          if (d.data) {

            if (this.scorllState == true)
              this.view.value.push(...d.data);
            else
              this.view.next(d.data);

            this._totalRecods = d.total;

            if (this.view.value) {
              this.recordInfoSubject.next(`Displaying ${this.view.value.length} of ${d.total} results`);
              this._skip = this._skip + this._take;
            }
            this.scorllState = false;
          }
        }
      });

    this._subsink.sink = this.connector
      .subscribe((param) => {

        if (param) {
          this.infiniteScroll.disabled = false;//set disabled to false to enable scroll to the next page.
          this.filterValue = param;
          this.resetScroll(); // reset _currentPage nad _totalRecords and its array container.

          this.data[this.method]({ // notify subscription.
            filter: {
              filters: this.filterValue,
              logic: "and"
            },
            skip: this._skip,
            take: this._take
          })
        }
      })
  }

  private _customDataTemplateContext: any = {};
  customDataTemplateContext(dataItem: any, index: number) {

    const context = this._customDataTemplateContext;

    context.$explicit = dataItem;
    context.dataItem = dataItem;
    context.index = index;
    context.context = this.view

    return context;
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
    this._timeout.forEach((timeout) => clearTimeout(timeout))
  }

  onClickDetail(dataItem: any) {

    this.cardCompnentTemplate.click.emit(dataItem);
  }

  clear() {

    this.view.next([]);
    this.recordInfoSubject.next(`Displaying ${0} of ${0} results`);
  }

  resetScroll() {

    this._skip = 0;
    this._currentPage = 1;
    this._totalRecods = 0;
  }

  scorllState = false;
  onInfiniteScroll(event: any) {

    this.scorllState = true;

    if (this._skip >= this._totalRecods) {
      this.infiniteScroll.disabled = true;
    } else {

      this._timeout.push(setTimeout(() => {
        this._currentPage++; // increase pager count.
        const state: State = { // notify subscription.
          filter: {
            filters: this.filterValue,
            logic: 'and'
          },
          skip: this._skip,
          take: this._take
        };
        this.data[this.method](state);
        event.target.complete();
      }, 500));
    }
  }
}
