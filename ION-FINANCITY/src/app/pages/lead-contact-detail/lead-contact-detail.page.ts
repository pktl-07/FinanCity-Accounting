import { Component, OnInit, OnDestroy, ViewChild, Inject } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ActionSheetController, Platform, } from '@ionic/angular';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { BroadcasterService } from 'src/app/services/broadcaster.service';
import { PropertyTransactionService } from 'src/app/services/property-transaction.service';
import { LeadService } from 'src/app/services/lead.service';
import { ShareContactsPage } from '../shared/share-contacts/share-contacts.page';
import { IFCConfig, SubSink } from 'src/@common/utils';
import { LocalNotifications, ILocalNotification } from '@ionic-native/local-notifications/ngx';
import { FCDateFormatPipe } from 'src/@common/pipes/fc-date-format.pipe';
import { FCTimeFormatPipe } from 'src/@common/pipes/fc-time-format.pipe';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'fc-lead-contact-detail',
  templateUrl: './lead-contact-detail.page.html',
  providers: [LeadService,
    FormUtilityService,
    LocalNotifications,
    FCTimeFormatPipe,
    FCDateFormatPipe,
    PropertyTransactionService]
})
export class LeadContactDetailPage implements OnInit, OnDestroy {

  public form: FormGroup;
  private _subsink = new SubSink();
  public remarkInsertedSubject = new BehaviorSubject([]);
  public remarkDeleteSubject = new BehaviorSubject([]);
  public shareKeySelection = [];

  @ViewChild('shareContact', { static: true }) _shareContact: ShareContactsPage;

  constructor(private _activeRoute: ActivatedRoute,
    private _leadService: LeadService,
    private _dateFormatPipe: FCDateFormatPipe,
    private _localNotification: LocalNotifications,
    private _timeFormatPipe: FCTimeFormatPipe,
    private _platform: Platform,
    private _broadcaster: BroadcasterService,
    private _formUtilityService: FormUtilityService,
    private _formBuilder: FormBuilder,
    private _actionSheetController: ActionSheetController) {

    this._subsink.sink = this._formUtilityService.OnSuccess
      .subscribe((data) => {

        if (data['Person']['RecordStatus'] != 0)
          this._broadcaster.broadcast('lead-reload');

        this._formUtilityService.rebind(this.form, data)
        this._shareContact.reBindKeyCollection(data['ShareContacts'])
        this.remarkInsertedSubject.next(data['Remarks'])

        if (data['Person']['RecordStatus'] == 1 || data['Person']['RecordStatus'] == 2)
          this.presentNotification({
            id: data['FollowUp']['SysKey'],
            beforeDay: data['FollowUp']['NotifyMeBeforeStartDate'],
            date: data['FollowUp']['StartDate'],
            name: data['Person']['Name'],
            time: data['FollowUp']['StartTime']
          })
      })
  }

  ngOnInit() {

    this._subsink.sink = this._activeRoute.data
      .subscribe((data) => {

        this.shareKeySelection.push(...data['client']['ShareContacts'])
        let c: any = this._formUtilityService.formGroupJson(data['client']);
        this.form = this._formBuilder.group(c);
        this.remarkInsertedSubject.next(data['client']['Remarks']);
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  action() {

    //cos two way databinding about getter and setter is not working.
    //still dont know why?
    const config = this._formUtilityService.build({
      form: this.form,
      status: +this.form.get('Person').get('RecordStatus').value,
      newHandler: this._leadService.default(),
      saveHandler: this._leadService.save({
        Lead: this.form.get('Lead').value,
        Person: this.form.get('Person').value,
        Contact: this.form.get('Contact').value,
        FollowUp: this.form.get('FollowUp').value
      }),
      updateHandler: this._leadService.update({
        Lead: this.form.get('Lead').value,
        Person: this.form.get('Person').value,
        Contact: this.form.get('Contact').value,
        FollowUp: this.form.get('FollowUp').value,
        InsertedSharedContacts: this._shareContact.getInsertedItem(),
        DeletedSharedContacts: this._shareContact.getDeletedItem(),
      }),
      deleteHandler: this._leadService.delete(this.form.value.Person.SysKey)
    })

    this._subsink.sink = config
      .subscribe(async (options) => {
        if (options.valid) {
          const actionSheet = await this._actionSheetController.create(options.config);
          await actionSheet.present();
        }
      })
  }

  //to present for device like push notification
  //on specify time.
  private presentNotification(param: {
    id: number,
    name: string,
    date: number,
    time: string,
    beforeDay: number,
  }) {

    const d = this._dateFormatPipe.transform(param.date, 'yyyymmdd');
    const t = this._timeFormatPipe.transform(param.time);
    const a = new Date(d);

    a.setTime(new Date().getTime()); // set current time.
    a.setDate(a.getDate() - param.beforeDay); //reduce before day.

    const trigger = new Date(a.getTime() + 3000); // wait 3s for device ready.

    const opt: ILocalNotification = {
      id: param.id,
      text: `You need to follow up ${param.name} on ${d} at ${t}`,
      trigger: { at: trigger },
      icon: `${environment.auth.api}/App_logo/logo.png`
    }

    if (this._platform.is('cordova'))
      this._localNotification.schedule(opt)
  }
}
