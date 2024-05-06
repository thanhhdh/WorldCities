import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { LoginResult } from './login-result';
import { AuthService } from './auth.service';
import { BaseFormComponent } from '../../base-form.component';
import { LoginRequest } from './login-request';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent
  extends BaseFormComponent implements OnInit {

  title?: string;
  loginResult?: LoginResult;
  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) { super(); }

  ngOnInit() {
    this.form = new FormGroup({
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
    });
  }

  onSubmit() {
    var loginRequest = <LoginRequest>{};
    loginRequest.email = this.form.controls['email'].value;
    loginRequest.password = this.form.controls['password'].value;

    this.authService.login(loginRequest)
      .subscribe(result => {
      console.log(result);
      this.loginResult = result;
      if (result.success) {
        this.router.navigate(["/"]);
      }
    }, error => {
      console.error(error);
      if (error.status == 401) {
        this.loginResult = error.error;
      }
    });
  }

}
