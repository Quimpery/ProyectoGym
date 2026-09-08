import { Component, OnInit } from '@angular/core';
import { ejercicioServices } from '../../services/ejercicioServices';

@Component({
  selector: 'app-ejercicios',
  imports: [],
  templateUrl: './ejercicios.html',
  
})
export class Ejercicios implements OnInit {

  listaEjercicios: any[] = [];

  constructor(private  ejercicioServices:ejercicioServices){}

  ngOnInit(): void {
    this.verEjercicios();
  }

  verEjercicios(){
    this.ejercicioServices.getEjerciciosRutina().subscribe({
      next : (data)=>{
        this.listaEjercicios = data;
        console.log('Los ejercicios fueron recibidos correctamente:',data);
      
      },
      error: (pito)=>{
        console.error("Ocurrio un error inexperado al traer los ejercicios",pito)
      }
    })
  }

}
