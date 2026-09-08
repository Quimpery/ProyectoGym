import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import {Ejercicios} from './features/ejercicios/ejercicios';
import {Rutinas} from './features/rutinas/rutinas'


export const routes: Routes = [
    {path: 'home', component: Home},
    {path: 'ejercicios', component: Ejercicios},
    {path: 'rutinas', component: Rutinas},
    {path: '**', redirectTo: 'home'}
]

