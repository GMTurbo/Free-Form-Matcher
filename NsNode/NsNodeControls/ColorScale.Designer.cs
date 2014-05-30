namespace NsNodeControls
{
     partial class ColorScale
     {
          /// <summary>
          /// Required designer variable.
          /// </summary>
          private System.ComponentModel.IContainer components = null;

          /// <summary>
          /// Clean up any resources being used.
          /// </summary>
          /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
          protected override void Dispose(bool disposing)
          {
               if (disposing && (components != null))
               {
                    components.Dispose();
               }
               base.Dispose(disposing);
          }

          #region Windows Form Designer generated code

          /// <summary>
          /// Required method for Designer support - do not modify
          /// the contents of this method with the code editor.
          /// </summary>
          private void InitializeComponent()
          {
               this.panel1 = new System.Windows.Forms.Panel();
               this.textBox2 = new System.Windows.Forms.TextBox();
               this.textBox1 = new System.Windows.Forms.TextBox();
               this.button1 = new System.Windows.Forms.Button();
               this.label1 = new System.Windows.Forms.Label();
               this.SuspendLayout();
               // 
               // panel1
               // 
               this.panel1.Location = new System.Drawing.Point(0, 46);
               this.panel1.Name = "panel1";
               this.panel1.Size = new System.Drawing.Size(169, 347);
               this.panel1.TabIndex = 8;
               this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
               this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
               // 
               // textBox2
               // 
               this.textBox2.Location = new System.Drawing.Point(0, 399);
               this.textBox2.Name = "textBox2";
               this.textBox2.Size = new System.Drawing.Size(169, 20);
               this.textBox2.TabIndex = 7;
               this.textBox2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox2_KeyUp);
               // 
               // textBox1
               // 
               this.textBox1.Location = new System.Drawing.Point(-3, 24);
               this.textBox1.Name = "textBox1";
               this.textBox1.Size = new System.Drawing.Size(169, 20);
               this.textBox1.TabIndex = 6;
               this.textBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyUp);
               // 
               // button1
               // 
               this.button1.Location = new System.Drawing.Point(41, 425);
               this.button1.Name = "button1";
               this.button1.Size = new System.Drawing.Size(75, 23);
               this.button1.TabIndex = 9;
               this.button1.Text = "Restore Defaults";
               this.button1.UseVisualStyleBackColor = true;
               this.button1.Click += new System.EventHandler(this.button1_Click);
               // 
               // label1
               // 
               this.label1.AutoSize = true;
               this.label1.Location = new System.Drawing.Point(59, 5);
               this.label1.Name = "label1";
               this.label1.Size = new System.Drawing.Size(51, 13);
               this.label1.TabIndex = 10;
               this.label1.Text = "Gaussian";
               // 
               // ColorScale
               // 
               this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
               this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
               this.ClientSize = new System.Drawing.Size(169, 451);
               this.Controls.Add(this.label1);
               this.Controls.Add(this.button1);
               this.Controls.Add(this.panel1);
               this.Controls.Add(this.textBox2);
               this.Controls.Add(this.textBox1);
               this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
               this.Name = "ColorScale";
               this.Text = "ColorScaler";
               this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorScale_FormClosing);
               this.ResumeLayout(false);
               this.PerformLayout();

          }

          #endregion

          private System.Windows.Forms.Panel panel1;
          private System.Windows.Forms.TextBox textBox2;
          private System.Windows.Forms.TextBox textBox1;
          private System.Windows.Forms.Button button1;
          private System.Windows.Forms.Label label1;
     }
}