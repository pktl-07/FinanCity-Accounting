import { Component, OnInit, ViewChild, OnDestroy, Inject } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject, of } from 'rxjs';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { createJsonFormGroup, makeFormTouched, rankType, SubSink } from 'src/@common/service/utility-service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { map, mergeMap, finalize, tap } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { CommissionStructureDetailService } from 'src/@sales/services/commission-structure-detail.service';
import { BranchOutService } from 'src/@sales/services/branch-out.service';
import { FileService } from 'src/@common/service/file.service';

@Component({
  selector: 'app-branch-out-detail',
  templateUrl: './branch-out-detail.component.html',
  providers: [
    BranchOutService,
    SalespersonManagementService,
    CommissionStructureDetailService,
    FileService,
    { provide: 'salesperson-1', useClass: SalespersonManagementService },
    { provide: 'salesperson-2', useClass: SalespersonManagementService },
  ], styles: [
    `
      .image-container {
        position: relative;
        width:100%;
      }
      .image-container > img {
        width:50px;
        height:50px;
      }
    `
  ]
})
export class BranchOutDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;

  _rank = new BehaviorSubject(rankType());
  _downTree = new BehaviorSubject([]);
  expandedKeys: any[] = [0];

  public salespersonDefaultItem: any = { SysKey: null, Name: 'N/A' };
  public nodeLength: number = 0;
  public maxMB: number;

  private _subsink = new SubSink();

  constructor(
    fb: FormBuilder,
    public _router: Router,
    private _activatedRoute: ActivatedRoute,
    private _branchOutService: BranchOutService,
    private _commissionStructure: CommissionStructureDetailService,
    private _fileService: FileService,
    @Inject('salesperson-1') public _salespersonManagementService1: SalespersonManagementService,
    @Inject('salesperson-2') public _salespersonManagementService2: SalespersonManagementService,
  ) {

    super(fb);

    this._subsink.sink = this._downTree
      .subscribe((node: any) => {

        this.nodeLength = 0;
        this.expandAll(node);
      });
  }

  ngOnInit() {

    this._toolbar
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['RecordStatus'])
      .NavigationButton({ action: () => this._router.navigateByUrl('/Management/SalespersonManagement/BranchOut') })
      .NewButton({
        action: () =>
          this._branchOutService.default().pipe(
            finalize(() => {

              this._downTree.next([])
              this.form.get('NRIC').patchValue(null);
              this.form.get('Designation').patchValue(null);
            })
          )
      })
      .SaveButton({
        action: () =>
          this._branchOutService.save(this.form.value),
        validate: () => {
          makeFormTouched(this.form);
          return this.form.valid
        }
      });

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {
        this.loadData(data['data']);
      });

    this._salespersonManagementService1.asyncFinds();
    this._salespersonManagementService2.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }

  private loadData(data: any): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {
      try {

        const c: any = createJsonFormGroup(data);
        this.form = this.fb.group(c);

        this.nodeLength = 0;
        this._downTree.next(data['nodes']);
        this.expandAll(data['nodes']);

        this._subsink.sink = this.form.get('SPSK').valueChanges
          .subscribe(value => {

            if (value != null) {

              this._subsink.sink = this._salespersonManagementService1.findByKey(value)
                .pipe(
                  map(response => {

                    response['NRIC'] = response['Person']['NRIC'];
                    response['Rank'] = response['Employment']['Rank'];
                    response['T1'] = response['Employment']['L1'];
                    response['IsActive'] = response['Person']['IsActive'];
                    response['ReasonToTerminate'] = response['Person']['ReasonToTerminate'];

                    return response;
                  }),
                  mergeMap(response => {
                    return this._commissionStructure.findByKey(response['Employment']['SchemeDetailSysKey'])
                      .pipe(
                        map(data => {

                          response['SchemeDetailSK'] = data['SysKey'];
                          response['Designation'] = data['Designation'];

                          return response;
                        }))
                  })
                ).subscribe((data: any) => {

                  this.form.get('IsActive').patchValue(data['IsActive']);
                  this.form.get('ReasonToTerminate').patchValue(data['']);
                  this.form.get('Rank').patchValue(data['Rank']);
                  this.form.get('SchemeDetailSK').patchValue(data['SchemeDetailSK']);
                  this.form.get('NRIC').patchValue(data['NRIC']);
                  this.form.get('Designation').patchValue(data['Designation']);
                  this.form.get('T1').patchValue(data['T1'], { emitEvent: false });
                });

              this._subsink.sink = this.form.get('T1').valueChanges
                .subscribe(value => {
                  this.onChangeT1(value);
                });
            }
          });

        this._subsink.sink = this.form.get('T1').valueChanges
          .subscribe(value => {

            this.onChangeT1(value);
          })

        this.form.get('SPSK').setValidators([Validators.required]);
        this.form.get('CreateDate').setValidators([Validators.required]);
        this.form.get('ApprovedBy').setValidators([Validators.required]);
        this.form.get('EffectiveDate').setValidators([Validators.required]);

        resolve(null);

      } catch (error) {
        reject(error)
      }
    })
  }

  public expandAll(node: []) {

    this.nodeLength++;
    node.map((o: any, i) => {

      this._subsink.sink = this._fileService.url({ fileType: 1, uploadId: o['SPSK'] })
        .pipe(
          tap((response) => {
            o['PhotoPath'] = response;
          }),
          finalize(() => {

            this.expandedKeys.push(node[i]['Depth']);
            if (o['Items'] && o['Items']['length'] > 0) {
              this.expandAll(o['Items']);
            }
          })
        ).subscribe()
    })
  }

  hasChildren = (item: any) => item.Items && item.Items.length > 0;

  fetchChildren = (item: any) => of(item.Items);

  getRankType(rank: number) {

    return rankType().find(x => x.value == rank).text;
  }

  private onChangeT1(key: string) {

    this._subsink.sink = this._salespersonManagementService1.getSalespersonFullTree(key)
      .subscribe((d: []) => {

        this.nodeLength = 0;
        this._downTree.next(d);
        this.expandAll(d);
      });
  }
}
