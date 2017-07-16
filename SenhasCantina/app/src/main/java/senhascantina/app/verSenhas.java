package senhascantina.app;

import senhascantina.app.util.SystemUiHider;

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.app.DialogFragment;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.TextView;
import android.support.v4.app.FragmentManager;
import android.app.AlertDialog;
import android.content.DialogInterface;

public class verSenhas extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //Remove title bar
        this.requestWindowFeature(Window.FEATURE_NO_TITLE);

        //Remove notification bar
        this.getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setContentView(R.layout.activity_ver_senhas);
        imprimirSenhas();
    }

    private void imprimirSenhas() {

        String sEst = Integer.toString(MainActivity.senhaEstudante);
        Button nEstudante = (Button)findViewById(R.id.bSE);
        nEstudante.setText(sEst);

        nEstudante.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(MainActivity.senhaEstudante==0)
                {
                    try {
                        showErrorDialog();
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
                else {
                    try {
                        showDialog("Estudante");
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            }
        });

        Button nFuncionario = (Button)findViewById(R.id.bSF);
        String sFun = Integer.toString(MainActivity.senhaFuncionario);
        nFuncionario.setText(sFun);

        nFuncionario.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(MainActivity.senhaFuncionario==0)
                {
                    try {
                        showErrorDialog();
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
                else {
                    try {
                        showDialog("Funcionario");
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            }
        });

        Button nVisitante = (Button)findViewById(R.id.bSV);
        String sVis = Integer.toString(MainActivity.senhaVisitante);
        nVisitante.setText(sVis);

        nVisitante.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(MainActivity.senhaVisitante==0)
                {
                    try {
                        showErrorDialog();
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
                else {
                    try {
                        showDialog("Visitante");
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            }
        });

        Button nExtra = (Button)findViewById(R.id.bSEx);
        String sExt = Integer.toString(MainActivity.senhaExtra);
        nExtra.setText(sExt);

        nExtra.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(MainActivity.senhaExtra==0)
                {
                    try {
                        showErrorDialog();
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
                else {
                    try {
                        showDialog("Extra");
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            }
        });

    }

    public void showErrorDialog() throws Exception{
        AlertDialog.Builder builder = new AlertDialog.Builder(verSenhas.this);

        builder.setMessage(R.string.sem_senha);

        builder.setPositiveButton(R.string.ok, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
            }
        });
        builder.show();
    }

    public void showDialog(final String senha) throws Exception
    {
        AlertDialog.Builder builder = new AlertDialog.Builder(verSenhas.this);

        builder.setMessage(R.string.confirmar_senha);

        builder.setPositiveButton(R.string.sim, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                try {
                    qRCode(senha);
                } catch (Exception e) {
                    e.printStackTrace();
                }

                dialog.dismiss();
            }
        });

        builder.setNegativeButton(R.string.nao, new DialogInterface.OnClickListener()
        {
            @Override
            public void onClick(DialogInterface dialog, int which)
            {
                dialog.dismiss();
            }
        });

        builder.show();
    }

    public void qRCode(final String senha) throws Exception
    {
        AlertDialog.Builder builder = new AlertDialog.Builder(verSenhas.this);
        builder.setCancelable(true);
        if(senha.equals("Estudante"))
        {
            builder.setIcon(R.drawable.qrcode_estudante);
            builder.setInverseBackgroundForced(true);
            builder.setTitle(" ");
            MainActivity.senhaEstudante--;
        }
        else if(senha.equals("Funcionario"))
        {
            builder.setIcon(R.drawable.qrcode_funcionario);
            builder.setInverseBackgroundForced(true);
            builder.setTitle(" ");
            MainActivity.senhaFuncionario--;
        }
        else if(senha.equals("Visitante"))
        {
            builder.setIcon(R.drawable.qrcode_visitante);
            builder.setInverseBackgroundForced(true);
            builder.setTitle(" ");
            MainActivity.senhaVisitante--;
        }
        else if(senha.equals("Extra"))
        {
            builder.setIcon(R.drawable.qrcode_extra);
            builder.setInverseBackgroundForced(true);
            builder.setTitle(" ");
            MainActivity.senhaExtra--;
        }

        builder.setPositiveButton(R.string.fechar_senha, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                startActivity(new Intent(verSenhas.this, senhas.class ));
            }
        });
        builder.show();
    }

    @Override
    public void onBackPressed() {
        startActivity(new Intent(verSenhas.this, senhas.class ));
    }
}


