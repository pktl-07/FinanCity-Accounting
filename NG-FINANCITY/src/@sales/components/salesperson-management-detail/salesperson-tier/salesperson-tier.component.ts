import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { CommissionStructureService } from 'src/@sales/services/commission-structure.service';
import { finalize, tap } from 'rxjs/operators';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { kendoFilter, rankType, SubSink } from 'src/@common/service/utility-service';
import { CommissionStructureBaseService } from 'src/@sales/services/commission-sturcutre-base.service';
import { FileService } from 'src/@common/service/file.service';
import { of } from 'rxjs';

@Component({
  selector: 'app-salesperson-tier',
  templateUrl: './salesperson-tier.component.html',
  styles: [
    `
      .image-container{
        position: relative;
        width:100%;
      }
      .image-container > img{
        width:50px;
        height:50px;
      }
    `
  ],
  providers: [
    SalespersonManagementService,
    FileService,
    CommissionStructureService,
    CommissionStructureBaseService
  ]
})
export class SalespersonTierComponent implements OnInit, OnDestroy {

  @Input() formGroup: FormGroup;

  _upTree = [];

  public defaultItem: any = { SysKey: null, Name: "N/A" };
  private _subsink = new SubSink();

  constructor(public fb: FormBuilder,
    public _fileService: FileService,
    public _commScheme: CommissionStructureService,
    public _salespersonService: SalespersonManagementService,
    public _commissionStructureBaseService: CommissionStructureBaseService) {
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  public filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  }

  ngOnInit(): void {

    if (this.formGroup.get('SysKey').value) {
      this._salespersonService.asyncFinds({
        filter: {
          logic: 'and',
          filters: kendoFilter([
            {
              field: 'SysKey', value: this.formGroup.get('SysKey').value, logic: 'and', operator: 'neq'
            }
          ])
        }
      })
    }
    else {
      this._salespersonService.asyncFinds();
    }

    this._subsink.sink = this._salespersonService.getSalespersonFullTree(this.formGroup.get('SysKey').value)
      .subscribe((d: []) => {

        this._upTree = d;
        this.expandAll(d);
      });

    this._subsink.sink = this._commScheme.findByKey(this.formGroup.get('SalespersonCommScheme').value)
      .subscribe((d: any) => {
        this.maxMB = d['MaxMB'];
      });
  }

  public maxMB: number;
  public nodeLength: number = 0;

  public expandAll(node: []) {

    this.nodeLength++;
    node.map((o: any, i) => {

      this._subsink.sink = this._fileService.url({ fileType: 1, uploadId: o['SPSK'] })
        .pipe(
          tap((response) => {
            o['PhotoPath'] = response;
            const reader = new FileReader();
            // reader.addEventListener('load', () => {
            //   o['PhotoPath'] = <string>reader.result;
            // })
            // reader.readAsDataURL(response);
          }),
          finalize(() => {

            this.expandedKeys.push(node[i]['Depth']);
            if (o['Items'] && o['Items']['length'] > 0) {
              this.expandAll(o['Items']);
            }
          })
        )
        .subscribe()
    })
  }

  hasChildren = (item: any) => item.Items && item.Items.length > 0;

  fetchChildren = (item: any) => of(item.Items);

  expandedKeys: any[] = [0];

  onChangeT1(key: string) {

    this._subsink.sink = this._salespersonService.getSalespersonFullTree(key)
      .subscribe((d: []) => {

        this.nodeLength = 0;
        this._upTree = d;
        this.expandAll(d);
      });
  }

  getRankType(rank: number) {

    return rankType().find(x => x.value == rank).text;
  }

  // getImage(path: string) {
  //   this._photoSerivce.getImage(path)
  //     .pipe(takeUntil(this._unsubscribe))
  //     .subscribe(response => {
  //       return new BehaviorSubject(`data:image/png;base64,${response}`);
  //     })
  // }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {
    return !itemArgs.dataItem.IsActive; //disable the 3rd item
  }
}
