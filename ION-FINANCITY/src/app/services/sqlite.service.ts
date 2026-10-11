import { Injectable } from '@angular/core';
import { SQLite, SQLiteObject } from '@ionic-native/sqlite/ngx';

@Injectable({ providedIn: 'root' })
export class SQLiteService {

    databaseObject: SQLiteObject;
    constructor(private sql: SQLite) {
    }

    createDb() {
        return new Promise((resolve, reject) => {
            return this.sql.create({
                name: 'SQLite',
                location: 'default'
            }).then((obj: SQLiteObject) => {
                this.databaseObject = obj;
                resolve(null)
            }).catch((error) => reject("Error creating on SQLite."));
        })
    }

    createTable() {
        return new Promise((resolve, reject) => {
            this.databaseObject.executeSql(`CREATE TABLE IF NOT EXISTS Login (id INTEGER PRIMARY KEY AUTOINCREMENT, email VARCHAR(255) NOT NULL UNIQUE)`,
                []).then(() => {
                    resolve(null)
                }).catch(() => reject("Error creating on SQLite Table"));
        })
    }

    addEmail(email: string) {
        return new Promise((resolve, reject) => {
            this.databaseObject.executeSql(
                `INSERT INTO Login (email) VALUES ('${email}')`, [])
                .then(() => {
                    resolve("Done creating data.");
                }).catch((e) => {
                    if (e.code === 6)
                        reject("email already exist.");

                    reject("error on creating email " + JSON.stringify(e));
                })
        })
    }

    updateEmail(email: string) {
        return new Promise((resolve, reject) => {
            this.databaseObject.executeSql(
                `UPDATE Login SET email='${email}'`, [])
                .then(() => {
                    resolve("Done Updating data.");
                }).catch((e) => {
                    if (e.code === 6)
                        reject("email already exist.");

                    reject("error on updating email " + JSON.stringify(e));
                })
        })
    }

    getEmail(): Promise<any> {
        return new Promise((resolve, reject) => {
            this.databaseObject.executeSql(`SELECT * FROM Login`, [])
                .then((data) => {
                    let result = []
                    if (data.rows?.length)
                        result.push(data.rows.item(0).email)
                    resolve(result)
                }).catch((e) => {
                    reject("error on getting email " + JSON.stringify(e))
                });
        })
    }

    keepCreditial(email: string): Promise<string> {
        return new Promise(async (resolve, reject) => {
            const data = await this.getEmail();
            if (data.length == 0) {
                this.addEmail(email).then(() => {
                    resolve(email)
                }).catch((error) => reject(error))
            }
            else if (typeof email == 'undefined' || email == null || email == '') {
                resolve(data[0])
            } else {
                this.updateEmail(email).then(() => {
                    resolve(email)
                }).catch((error) => reject(error))
            }
        })
    }
}