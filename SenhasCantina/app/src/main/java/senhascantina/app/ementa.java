package senhascantina.app;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Window;
import android.view.WindowManager;
import java.util.ArrayList;
import java.util.List;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.Toast;

public class ementa extends Activity {

    static String ementa1 = "Sopa - Sopa de brócolos \n Prato Carne - Costelinha com molho barbecue e feijão preto \n Prato Dieta - Badejo assado com maionese e batata cozida \n Prato de Peixe - Badejo grelhado com batata e cenoura cozida \n Prato Dieta - Badejo grelhado com batata e cenoura cozida";
    //static String ementa2 = {"Sopa de brócolos","Frango grelhado com massa esparguete","","Arroz à valenciana","","Buffet de saladas","Pão de mistura","Fruta da época ou doce","Água mineral"};
    //static String ementa3 = {"Sopa de brócolos","Esparguete à bolonhesa","","Caril de tofu","","Buffet de saladas","Pão de mistura","Fruta da época ou doce","Água mineral"};
    //static String ementa4 = {"Sopa de brócolos","Perna de peru assada com massa","Pescada cozida com todos (batata, couves, cenoura e ovo)","","","Buffet de saladas","Pão de mistura","Fruta da época ou doce","Água mineral"};
    static String ementaJantarCrasto = "Encerrado - refeições servidas no refeitório de Santiago";

    private Spinner spinner1, spinner2, spinner3;
    private Button btnSubmit;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //Remove title bar
        this.requestWindowFeature(Window.FEATURE_NO_TITLE);

        //Remove notification bar
        this.getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setContentView(R.layout.activity_ementa);
        addListenerOnButton();
        montarSpinners();
    }

    @Override
    public void onBackPressed() {
        startActivity(new Intent(ementa.this, MainActivity.class ));
    }

    public void montarSpinners() {
        spinner1 = (Spinner) findViewById(R.id.spinner1);
        spinner2 = (Spinner) findViewById(R.id.spinner2);
        spinner3 = (Spinner) findViewById(R.id.spinner3);
    }

    // get the selected dropdown list value
    public void addListenerOnButton() {

        spinner1 = (Spinner) findViewById(R.id.spinner1);
        spinner2 = (Spinner) findViewById(R.id.spinner2);
        spinner3 = (Spinner) findViewById(R.id.spinner3);
        btnSubmit = (Button) findViewById(R.id.btnSubmit);

        btnSubmit.setOnClickListener(new OnClickListener() {

            @Override
            public void onClick(View v) {

                showEmenta();
            }
        });
    }
    public void showEmenta()
    {
        AlertDialog.Builder builder = new AlertDialog.Builder(ementa.this);
        LayoutInflater inflater = ementa.this.getLayoutInflater();

        if(spinner1.getSelectedItem().equals("Crasto") && spinner3.getSelectedItem().equals("Jantar"))
            builder.setTitle(ementaJantarCrasto);
        else
            builder.setView(inflater.inflate(R.layout.ementa_dialog, null));

        builder.setPositiveButton("Fechar Janela", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
            }
        });
        builder.show();
    }
}