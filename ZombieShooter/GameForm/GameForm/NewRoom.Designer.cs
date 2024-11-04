﻿namespace GameForm
{
    partial class NewRoom
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
            this.label1 = new System.Windows.Forms.Label();
            this.createButton = new System.Windows.Forms.Button();
            this.maPhong = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.joinButton = new System.Windows.Forms.Button();
            this.listPhong = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.showRoomList = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(-4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "Xem các phòng hiện có:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // createButton
            // 
            this.createButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createButton.Location = new System.Drawing.Point(92, 121);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(102, 37);
            this.createButton.TabIndex = 1;
            this.createButton.Text = "Tạo phòng";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // maPhong
            // 
            this.maPhong.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maPhong.Location = new System.Drawing.Point(244, 56);
            this.maPhong.Multiline = true;
            this.maPhong.Name = "maPhong";
            this.maPhong.Size = new System.Drawing.Size(121, 30);
            this.maPhong.TabIndex = 29;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(72, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(158, 31);
            this.label7.TabIndex = 28;
            this.label7.Text = "Nhập mã phòng:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // joinButton
            // 
            this.joinButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.joinButton.Location = new System.Drawing.Point(237, 121);
            this.joinButton.Name = "joinButton";
            this.joinButton.Size = new System.Drawing.Size(102, 37);
            this.joinButton.TabIndex = 31;
            this.joinButton.Text = "Tham gia";
            this.joinButton.UseVisualStyleBackColor = true;
            this.joinButton.Click += new System.EventHandler(this.joinButton_Click);
            // 
            // listPhong
            // 
            this.listPhong.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listPhong.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listPhong.FormattingEnabled = true;
            this.listPhong.Location = new System.Drawing.Point(244, 11);
            this.listPhong.Name = "listPhong";
            this.listPhong.Size = new System.Drawing.Size(121, 27);
            this.listPhong.TabIndex = 32;
            this.listPhong.SelectedIndexChanged += new System.EventHandler(this.listPhong_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GameForm.Properties.Resources.grave;
            this.pictureBox1.Location = new System.Drawing.Point(365, 109);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(70, 70);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 33;
            this.pictureBox1.TabStop = false;
            // 
            // showRoomList
            // 
            this.showRoomList.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showRoomList.Location = new System.Drawing.Point(372, 11);
            this.showRoomList.Name = "showRoomList";
            this.showRoomList.Size = new System.Drawing.Size(58, 27);
            this.showRoomList.TabIndex = 34;
            this.showRoomList.Text = "Xem ";
            this.showRoomList.UseVisualStyleBackColor = true;
            this.showRoomList.Click += new System.EventHandler(this.showRoomList_Click);
            // 
            // NewRoom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 189);
            this.Controls.Add(this.showRoomList);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listPhong);
            this.Controls.Add(this.joinButton);
            this.Controls.Add(this.maPhong);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.label1);
            this.Name = "NewRoom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NewRoom";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewRoom_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.TextBox maPhong;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button joinButton;
        private System.Windows.Forms.ComboBox listPhong;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button showRoomList;
    }
}