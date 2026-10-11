import { FormBuilder, FormGroup, FormControl, FormArray } from '@angular/forms';
import { Observable, of, Subject } from 'rxjs';
import { Injectable, OnDestroy } from '@angular/core';
import { AlertController, LoadingController, ToastController } from '@ionic/angular';
import { ActionSheetOptions, ActionSheetButton } from '@ionic/core';
import { tap } from 'rxjs/operators';
import { SubSink } from 'src/@common/utils';

@Injectable({ providedIn: 'root' })
export class FormUtilityService implements OnDestroy {

    public fb: FormBuilder;
    public form: FormGroup;
    public OnSuccess = new Subject();
    private _actionType: string;
    private _subsink = new SubSink();

    constructor(formBuilder: FormBuilder,
        private _toastCtrl: ToastController,
        private _loadingCtrl: LoadingController,
        private _alertCtrl: AlertController) {

        this.fb = formBuilder;
        this.form = this.fb.group({});
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    public formGroupJson(input: any,
        g?: FormGroup,
        appendInGroup?: (s: { form: FormGroup, name: string }) => any,
        blur: string[] = []): {} {

        let d = {};
        Object.getOwnPropertyNames(input).forEach(f => {
            if (Array.isArray(input[f]) == false && typeof input[f] == "object" && input[f] != null) {
                let n = new FormGroup({});
                d[f] = n;

                if (appendInGroup)
                    appendInGroup({ form: n, name: f });

                this.formGroupJson(input[f], n, appendInGroup, blur);
            } else {

                if (g != undefined) {
                    let form: FormControl;
                    if (blur.includes(f, 0))
                        form = new FormControl(input[f], { updateOn: 'blur' });
                    else
                        form = new FormControl(input[f]);
                    g.addControl(f, form);

                } else {
                    let form: FormControl;
                    if (blur.includes(f, 0))
                        form = new FormControl(input[f], { updateOn: 'blur' });
                    else
                        form = new FormControl(input[f]);
                    d[f] = form;
                }
            }
        });
        return d;
    }

    public makeFormTouched(group: FormGroup | FormArray) {

        Object.keys(group.controls).forEach((key: string) => {
            const abstractControl = group.controls[key];
            if (abstractControl instanceof FormGroup || abstractControl instanceof FormArray) {
                this.makeFormTouched(abstractControl);
            } else {
                let formControl = abstractControl as FormControl;
                formControl.markAsTouched();
            }
        });
    }

    public rebind(group: FormGroup | FormArray, data: any) {

        Object.keys(group.controls).forEach((key: string) => {

            const abstractControl = group.controls[key];
            if (abstractControl instanceof FormGroup || abstractControl instanceof FormArray) {
                this.rebind(abstractControl, data[key]);
            } else {

                let formControl = abstractControl as FormControl;
                if (Object.keys(data).filter(x => x == key).length > 0) {
                    formControl.patchValue(data[key])
                } else {

                    if (this._actionType == 'create new') {
                        formControl.patchValue(null);
                    }
                }
            }
        });
    }

    public action(handler: Observable<any>, type: 'save' | 'update' | 'delete' | 'change password') {

        this.animate({
            handler: handler.pipe(tap((result) => {
                this.createToast(type)
                    .then((d) => {
                        d.present()
                            .then(() => {
                                this.OnSuccess.next(result);
                            })
                    });
            })),
            type: type
        })
    }

    build(options: {
        form: FormGroup,
        status: number,
        resubmitHandler?: Observable<any>,
        saveDraftHandler?: Observable<any>,
        submitDraftHandler?: Observable<any>,
        newHandler?: Observable<any>,
        saveHandler?: Observable<any>,
        updateHandler?: Observable<any>,
        deleteHandler?: Observable<any>
    }): Observable<{ config: ActionSheetOptions, valid: boolean }> {

        this.makeFormTouched(options.form);
        if (options.form.valid == false) {

            this._alertCtrl.create({
                header: 'Warning',
                message: `Validation has failed. \n\n
                Please check through the form again.`,
                subHeader: '',
                buttons: [
                    {
                        text: 'OK',
                        role: 'cancel'
                    }
                ]
            }).then((a) => {
                a.present().then();
            })
        } else {

            const header = 'actions';
            const newConfig: ActionSheetButton = {
                text: 'New',
                icon: 'add',
                handler: () => {
                    this.animate({
                        handler: options.newHandler.pipe(tap((result) => {
                            this.OnSuccess.next(result);
                        })),
                        type: 'create new'
                    })
                }
            };

            const saveDraftConfig: ActionSheetButton = {
                text: 'Save Draft',
                icon: 'save',
                handler: () => {
                    this.animate({
                        handler: options.saveDraftHandler.pipe(tap((result) => {
                            this.createToast('save draft').then((d) => {
                                d.present().then(() => {
                                    this.OnSuccess.next(result);
                                })
                            });
                        })),
                        type: 'save draft'
                    })
                }
            };

            const submitConfig: ActionSheetButton = {
                text: 'Submit',
                icon: 'arrow-up',
                handler: () => {
                    this.animate({
                        handler: options.submitDraftHandler.pipe(tap((result) => {
                            this.createToast('submit').then((d) => {
                                d.present().then(() => {
                                    this.OnSuccess.next(result);
                                })
                            });
                        })),
                        type: 'submit'
                    })
                }
            };

            const reSubmitConfig: ActionSheetButton = {
                text: 'Re-Submit',
                icon: 'save',
                handler: () => {
                    this.animate({
                        handler: options.resubmitHandler.pipe(tap((result) => {
                            this.createToast('resubmit').then((d) => {
                                d.present().then(() => {
                                    this.OnSuccess.next(result);
                                })
                            });
                        })),
                        type: 'resubmit'
                    })
                }
            };

            const saveConfig: ActionSheetButton = {
                text: 'Save',
                icon: 'save',
                handler: () => {
                    this.animate({
                        handler: options.saveHandler.pipe(tap((result) => {
                            this.createToast('save').then((d) => {
                                d.present().then(() => {
                                    this.OnSuccess.next(result);
                                })
                            });
                        })),
                        type: 'save'
                    })
                }
            };

            const updateConfig: ActionSheetButton = {
                text: 'Update',
                icon: 'create',
                handler: () => {
                    this.animate({
                        handler: options.updateHandler.pipe(tap((result) => {
                            this.createToast('update').then((d) => {
                                d.present().then(() => {
                                    this.OnSuccess.next(result);
                                })
                            });
                        })),
                        type: 'update'
                    })
                }
            };

            const deleteConfig: ActionSheetButton = {
                text: 'Delete',
                icon: 'trash',
                handler: () => {
                    this.animate({
                        handler: options.deleteHandler.pipe(tap((result) => {
                            this.createToast('delete').then((d) => {
                                d.present().then(() => {
                                    this.OnSuccess.next(result);
                                })
                            });
                        })),
                        type: 'delete'
                    })
                }
            };

            const cancelConfig: ActionSheetButton = {
                text: 'Cancel',
                role: 'cancel',
                icon: 'close',
            };

            switch (options.status) {
                case -10:
                    return of({
                        config: {
                            header: header,
                            buttons: [
                                saveDraftConfig,
                                cancelConfig
                            ]
                        }, valid: this.form.valid
                    })
                case -2:
                    return of({
                        config: {
                            header: header,
                            buttons: [
                                newConfig,
                                reSubmitConfig,
                                cancelConfig,
                            ]
                        }, valid: this.form.valid
                    })
                case -3:
                    return of({
                        config: {
                            header: header,
                            buttons: [
                                newConfig,
                                submitConfig,
                                updateConfig,
                                deleteConfig,
                                cancelConfig
                            ]
                        }, valid: this.form.valid
                    })
                case 0:
                    return of({
                        config: {
                            header: header,
                            buttons: [
                                saveConfig,
                                cancelConfig
                            ]
                        }, valid: this.form.valid
                    })
                case 1:
                case 2:
                case 3:
                    return of({
                        config: {
                            header: header,
                            buttons: [
                                newConfig,
                                updateConfig,
                                deleteConfig,
                                cancelConfig
                            ]
                        }, valid: this.form.valid
                    })
                case 10:
                    return of({
                        config: {
                            header: header,
                            buttons: [
                                updateConfig,
                                cancelConfig
                            ]
                        }, valid: this.form.valid
                    })

                default:
                    return of({
                        config: {
                            header: header,
                            buttons: [
                                newConfig,
                                cancelConfig
                            ]
                        }, valid: this.form.valid
                    });
            }
        }
    }

    createToast(type: 'save' | 'update' | 'delete' | 'save draft' | 'submit' | 'resubmit' | 'change password') {
        return this._toastCtrl.create({
            message: type == 'change password' ? 'Your password has been changed.' : `Records has been ${type}.`,
            duration: 2000,
            color: 'dark',
        })
    }

    animate(options: {
        handler: Observable<any>,
        type: 'create new' | 'save' | 'update' | 'delete' | 'save draft' | 'submit' | 'resubmit' | 'change password'
    }) : void {
        this._actionType = options.type;
        this._alertCtrl.create({
            header: 'Confirm',
            message: `Are you sure you want to ${options.type}? ${options.type == 'create new' ? 'Any unsave data will be lost.' : ''}`,
            buttons: [{
                text: 'OK',
                handler: async () => {

                    const loading = await this._loadingCtrl.create({
                        message: 'Loading...'
                    });

                    //  await loading.present();
                    loading.present().then(async () => {

                        this._subsink.sink = options.handler
                            .subscribe(async () => {
                                await loading.dismiss();
                            }, async (err) => {

                                await loading.dismiss();
                                
                                let d = (err.error.Message as string).replace(/--(.*):/g, '*');
                                let m = d.split('*');
                                let message = '';
                                if (m) {
                                    if (m[0].indexOf('Validation failed') == 0) {
                                        m.shift();
                                    }
                                }
                                m.forEach((value, index) => {
                                    message += `
                                <ion-row nowrap align-items-start>
                                    <ion-col>
                                        ${index + 1}. ${value}
                                    </ion-col>
                                </ion-row>`
                                });
                                this._alertCtrl.create({
                                    header: 'Validation Errors',
                                    message: `<ion-grid>${message}</ion-grid>`,
                                    buttons: [{
                                        text: 'OK',
                                        role: 'Cancel',
                                    }]
                                }).then((d) => {
                                    d.present().then();
                                })
                            })
                    })
                },
            }, {
                text: 'Cancel',
                role: 'cancel'
            }],
        }).then((a) => {
            a.present();
        });
    }
}
