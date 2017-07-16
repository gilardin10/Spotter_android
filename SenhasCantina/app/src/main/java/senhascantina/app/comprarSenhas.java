package senhascantina.app;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class comprarSenhas extends Activity {


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //Remove title bar
        this.requestWindowFeature(Window.FEATURE_NO_TITLE);

        //Remove notification bar
        this.getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setContentView(R.layout.activity_comprar_senhas);
        comprar();
    }

    private void comprar() {
        Button btnCompra = (Button) findViewById(R.id.btnComprar);
        btnCompra.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Toast.makeText(comprarSenhas.this, "Compra efetuada com sucesso.", Toast.LENGTH_LONG).show();
                actualizaSenhas();
                startActivity(new Intent(comprarSenhas.this, senhas.class ));
            }
        });
    }

    public void actualizaSenhas()
    {
        EditText editEstudante = (EditText)findViewById(R.id.editEst);
        EditText editFuncionario = (EditText)findViewById(R.id.editFunc);
        EditText editVisitante = (EditText)findViewById(R.id.editVisi);
        EditText editExtra = (EditText)findViewById(R.id.editExt);
        if(!editEstudante.getText().toString().equals(""))
        {
            MainActivity.senhaEstudante += Integer.parseInt(editEstudante.getText().toString());
        }
        if(!editFuncionario.getText().toString().equals(""))
        {
            MainActivity.senhaFuncionario += Integer.parseInt(editFuncionario.getText().toString());
        }
        if(!editVisitante.getText().toString().equals(""))
        {
            MainActivity.senhaVisitante += Integer.parseInt(editVisitante.getText().toString());
        }
        if(!editExtra.getText().toString().equals(""))
        {
            MainActivity.senhaExtra += Integer.parseInt(editExtra.getText().toString());
        }

    }

    @Override
    public void onBackPressed() {
        startActivity(new Intent(comprarSenhas.this, senhas.class ));
    }

}
