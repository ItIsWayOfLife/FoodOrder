import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { User } from '../admin/users/user';
import { UserChangePassword } from '../admin/users/userChangePassword';

@Injectable()
export class UserService {

    private url = "https://localhost:44342/api/users";

    constructor(private http: HttpClient) { }

    getUsers() {
        return this.http.get(this.url);
    }

    createUser(user: User) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.post(this.url, JSON.stringify(user), { headers: myHeaders, observe: 'response' });
    }

    updateUser(user: User) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.put(this.url, JSON.stringify(user), { headers: myHeaders, observe: 'response' });
    }

    deleteUser(id: string) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.delete(this.url + '/' + id, { headers: myHeaders, observe: 'response' });
    }

    changePass(userChangePass: UserChangePassword) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.put(this.url + '/changepassword', JSON.stringify(userChangePass), { headers: myHeaders, observe: 'response' });
    }
}
