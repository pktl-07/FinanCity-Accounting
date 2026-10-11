import { DialogAction } from '@progress/kendo-angular-dialog';
import { ButtonControlConfig, IDataComplete } from 'src/@common/Infrastructure/common-interface';
import { PConfirmDialogComponent } from '../p-confirm-dialog/p-confirm-dialog.component';
import { Injectable, OnDestroy } from '@angular/core';
import { catchError, tap } from 'rxjs/operators';
import { Subject, from, throwError } from 'rxjs';
import { ToastService } from '../p-toast/p-toast.service';
import { SubSink } from 'src/@common/service/utility-service';

@Injectable({ providedIn: 'root' })
export class ButtonToolbarControlService implements OnDestroy {

    private _subsink = new SubSink();

    private _allowUpdateAfterPost: boolean;
    public get allowUpdateAfterPost(): boolean {
        return this._allowUpdateAfterPost;
    }
    public set allowUpdateAfterPost(value: boolean) {
        this._allowUpdateAfterPost = value;
    }

    constructor(public _toastService: ToastService) {
    }

    destory() {
        this._subsink.unsubscribe();
    }

    ngOnDestroy(): void {
        this.destory()
    }

    changeState(state: number = 0, config: Array<ButtonControlConfig>) {

        config.filter(x => x.label != 'navigate'
            && (typeof x.hide == 'undefined' || x.hide == false)).forEach(x => {
                switch (state) {
                    case -2:
                        if (x.label.toLowerCase() == 'new')
                            x.disabled = false;
                        else
                            x.disabled = true;
                        break;
                    case -1:
                        if (x.label.toLowerCase() == 'approve' ||
                            x.label.toLowerCase() == 'reject' ||
                            x.label.toLowerCase() == 'new')

                            x.disabled = false;
                        else
                            x.disabled = true;
                        break;
                    case 0:
                    case 1:
                        if (x.label.toLowerCase() == 'save' ||
                            x.label.toLowerCase() == 'download pdf' ||
                            x.label.toLowerCase() == 'download cvs' ||
                            x.label.toLowerCase() == 'new')

                            x.disabled = false;
                        else
                            x.disabled = true;

                        break;
                    case 2: //update state 
                    case 3: //update state
                        if (x.label.toLowerCase() == 'save' ||
                            x.label.toLowerCase() == 'approve' ||
                            x.label.toLowerCase() == 'reject' ||
                            x.label.toLowerCase() == 'reverse')

                            x.disabled = true;
                        else
                            x.disabled = false;
                        break;

                    case 6:
                    case 125:
                    case 126:
                        if (x.label.toLowerCase() == 'new')
                            x.disabled = false;
                        break;

                    case 8:

                        if (this.allowUpdateAfterPost == true) {

                            if (x.label.toLowerCase() == 'delete' ||
                                x.label.toLowerCase() == 'save' ||
                                x.label.toLowerCase() == 'print' ||
                                x.label.toLowerCase() == 'approve' ||
                                x.label.toLowerCase() == 'reject' ||
                                x.label.toLowerCase() == 'void' ||
                                x.label.toLowerCase() == 'post')
                                x.disabled = true;
                            else
                                x.disabled = false;
                        } else {

                            if (x.label.toLowerCase() == 'delete' ||
                                x.label.toLowerCase() == 'save' ||
                                x.label.toLowerCase() == 'print' ||
                                x.label.toLowerCase() == 'update' ||
                                x.label.toLowerCase() == 'approve' ||
                                x.label.toLowerCase() == 'reject' ||
                                x.label.toLowerCase() == 'void' ||
                                x.label.toLowerCase() == 'post')
                                x.disabled = true;
                            else
                                x.disabled = false;
                        }
                        break;
                    default: // delete or default
                        if (x.label.toLowerCase() == "update" ||
                            x.label.toLowerCase() == "delete")
                            x.disabled = true;
                        else
                            x.disabled = false;
                        break;
                }
            });
    }

    disableAll(config: Array<ButtonControlConfig>) {
        config.filter(x => x.label.toLocaleLowerCase() != 'navigate').forEach(x => {
            x.disabled = true;
        });
    }

    private _confirmDialog: PConfirmDialogComponent;

    action(confirmDialog: PConfirmDialogComponent,
        item: ButtonControlConfig,
        error: Subject<any>,
        toast: Subject<ButtonControlConfig>,
        warn?: any,
        fail?: any,
        success?: Subject<IDataComplete>,
        start?: Subject<any>,
        dirty?: boolean) {

        this._confirmDialog = confirmDialog;

        // let us choose for content template or string. 
        let content = item.label.toLowerCase() == 'new' ? warn ? warn : `Are you sure you want to create new.`
            : `Are you sure you want to ${item.label.toLowerCase()}.`;
        //to do the task.
        const action = () => {
            this.excuteAction(item, error, toast, success, start);// trigger action
        };
        //popping out the dialog and ask for excution the task.
        const _do = (_: () => any) => {
            this._subsink.sink = this._confirmDialog.open({ title: 'Confirm', content: content })
                .result
                .subscribe((d: DialogAction) => {
                    if (d.text.toLocaleLowerCase() == 'yes') {
                        _();
                    }
                });
        }

        //form validation.
        const validate: boolean = (typeof item.action.validate == undefined) ? true : item.action.validate();
        //combinding of all task such as vlidate and do action unless popping out the Validation fail dialog result.
        const serve = () => {
            if (validate)
                _do(action);
            else
                confirmDialog.open({
                    title: 'Warning',
                    content: fail,
                    action: [{ text: 'OK' }]
                });
        };

        if (item.label.toLowerCase() != "new")
            serve(); // serve the action such as check form validation and poping appropriate dialog box such as update, delete ect...
        else if (item.label.toLowerCase() == 'new' && dirty)
            serve(); // will do the same as above but when action is new and it's form is dirty then check through the validate again.
        else
            action(); // for new button click When form is not dirty.
    }

    private excuteAction(
        item: ButtonControlConfig,
        error: Subject<any>,
        toast: Subject<ButtonControlConfig>,
        success?: Subject<IDataComplete>,
        start?: Subject<any>) {

        start.next(true);

        this._subsink.sink = from(item.action.action()).pipe(
            catchError((e) => {
                return throwError(e);
            }), tap((data) => {
                // when done then notify success subject.
                // It is a trigger point to prepare action complete task such as change buttons control state or rebind the form.
                if (typeof success !== 'undefined' && item.label.toLowerCase() != 'recap')
                    success.next({ data: data, action: item.label.toUpperCase() });
                //To show bootstrap toast object.
                if (item.action.requireNotification)
                    toast.next(item);
            })).subscribe(() => { },
                (e: any) => { error.next(e) });
    }
}

