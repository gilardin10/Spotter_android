package senhascantina.app;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;

public class MainActivity extends Activity {

    static int senhaEstudante = 0;
    static int senhaFuncionario = 0;
    static int senhaVisitante = 0;
    static int senhaExtra = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //Remove title bar
        this.requestWindowFeature(Window.FEATURE_NO_TITLE);

        //Remove notification bar
        this.getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);
        setContentView(R.layout.activity_menu);

        setupButton();
    }

    private void setupButton() {
        Button btnTickets = (Button) findViewById(R.id.btnSenhas);
        btnTickets.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
               startActivity(new Intent(MainActivity.this, senhas.class ));
            }
        });

        Button btnCard = (Button) findViewById(R.id.btnEmenta);
        btnCard.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(MainActivity.this,ementa.class));
            }
        });

        Button btnSettings = (Button) findViewById(R.id.btnSettings);
        btnSettings.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(MainActivity.this, settings.class ));
            }
        });

        Button btnHelp = (Button) findViewById(R.id.btnAjuda);
        btnHelp.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(MainActivity.this, ajuda.class ));
            }
        });


    }

    @Override
    public void onBackPressed() {

    }
}
