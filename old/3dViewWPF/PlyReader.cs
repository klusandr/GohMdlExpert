using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace _3dViewWPF {
    public class PlyReader {
        private const int INDEX_SIZE = 6;

        private static readonly string[] s_chunkNames = { "SKIN", "MESH", "VERT", "INDX" };
        private static readonly int[] s_mashFormats = { 0x0644, 0x0604, 0x0404, 0x0704, 0x0744, 0x0C14 };


        public static List<Point3D> Points { get; set; } = new();
        public static List<Vector3D> Normalizes { get; set; } = new();
        public static List<int> IndicesList { get; set; } = new();

        public static void PlyRead (string fileName) {
            Points.Clear();
            Normalizes.Clear();
            IndicesList.Clear();

            using var modelFileStream = new FileStream(fileName, FileMode.Open);
            byte[] readBytes = new byte[128];

            modelFileStream.Read(readBytes, 0, 8);
            var fileHeader = Encoding.ASCII.GetString(readBytes, 0, 8);

            float[] readFloat = new float[6];
            modelFileStream.Read(readBytes, 0, 24);
            for (int i = 0; i < readFloat.Length; i++) {
                readFloat[i] = BitConverter.ToSingle(readBytes, i * 4);
            }

            while (modelFileStream.Position < modelFileStream.Length) {
                modelFileStream.Read(readBytes, 0, 4);

                string chunkName = Encoding.ASCII.GetString(readBytes, 0, 4);

                switch (chunkName) {
                    case "SKIN":
                        modelFileStream.Read(readBytes, 0, 4);
                        int skinCount = BitConverter.ToInt32(readBytes, 0);

                        for (int i = 0; i < skinCount; i++) {
                            int skinNameSize = modelFileStream.ReadByte();
                            modelFileStream.Read(readBytes, 0, skinNameSize);
                            string skinName = Encoding.ASCII.GetString(readBytes, 0, skinNameSize);
                        }
                        break;
                    case "MESH":
                        modelFileStream.Read(readBytes, 0, 8);

                        modelFileStream.Read(readBytes, 0, 8);
                        int triangelsCount = BitConverter.ToInt32(readBytes, 0);
                        int materialInfo = BitConverter.ToInt32(readBytes, 4);

                        if (materialInfo != 0x0404 && materialInfo != 0x0C14) {
                            //modelFileStream.Read(readBytes, 0, 4);
                        }

                        int materialNameSize = modelFileStream.ReadByte();

                        modelFileStream.Read(readBytes, 0, materialNameSize);
                        string materialName = Encoding.ASCII.GetString(readBytes, 0, materialNameSize);

                        for (int i = 0; i < 10; i++) {
                            if (modelFileStream.ReadByte() == 'V') {
                                break;
                            }
                        }

                        modelFileStream.Position--;

                        break;
                    case "VERT":
                        modelFileStream.Read(readBytes, 0, 8);
                        int vertexCount = BitConverter.ToInt32(readBytes, 0);
                        int vertexDataSize = BitConverter.ToInt16(readBytes, 4);
                        int vertexFlags = BitConverter.ToInt16(readBytes, 6);

                        byte[] vertexesData = new byte[vertexCount * vertexDataSize];
                        modelFileStream.Read(vertexesData, 0, vertexesData.Length); 

                        for (int i = 0; i < vertexCount; i++) {
                            byte[] vertexData = new byte[vertexDataSize];
                            Array.Copy(vertexesData, i * vertexDataSize, vertexData, 0, vertexDataSize);

                            var point = new Point3D {
                                X = BitConverter.ToSingle(vertexData, 0 * 4) * - 1,
                                Y = BitConverter.ToSingle(vertexData, 2 * 4),
                                Z = BitConverter.ToSingle(vertexData, 1 * 4)
                            };

                            var normal = new Vector3D {
                                X = BitConverter.ToSingle(vertexData, 3 * 4),
                                Y = BitConverter.ToSingle(vertexData, 5 * 4),
                                Z = BitConverter.ToSingle(vertexData, 4 * 4)
                            };

                            //point.X *= -1;

                            Points.Add(point);
                            Normalizes.Add(normal);
                        }

                        //switch (vertexType) {
                        //    case 65572:
                        //        break;
                        //    case 458784:
                        //        break;
                        //    case 0x00070028:
                        //        break;
                        //    case 0x00070030:
                        //        break;
                        //    default:
                        //        break;
                        //}

                        break;
                    case "INDX":
                        modelFileStream.Read(readBytes, 0, 4);
                        int indicesCount = BitConverter.ToInt32(readBytes, 0);

                        byte[] indicesData = new byte[indicesCount * 2];
                        modelFileStream.Read(indicesData, 0, indicesData.Length);

                        for (int i = 0; i < indicesCount; i++) {
                            IndicesList.Add(BitConverter.ToInt16(indicesData, i * 2));
                        }

                        for (int i = 0; i < IndicesList.Count; i += 3) {
                            var temp = IndicesList[i + 1];
                            IndicesList[i + 1] = IndicesList[i + 2];
                            IndicesList[i + 2] = temp;
                        }

                        break;
                    default: 
                        break;
                }
            }


        }
    }

}

//struct Tply_mesh_base {
//    DWORD fvf;
//    DWORD first_face;
//    DWORD face_count;
//    DWORD flags;
//};

//CPly::CPly(char * plyname)
//{
//    int tex_count = 1;
//    int i = 0, sigindx = 0, i2 = 0;
//    char texname[4][_MAX_PATH] = { 0}, textemp[_MAX_PATH] = { 0}, *path = NULL;
//    char text_msg_buff[2048] = { 0 };
//    bool skip = false, CopyUV = false;
//    BYTE sig[5] = { 0 };
//    Tply_mesh_base ply_mesh;
//    memset(&ply_mesh, 0, sizeof(Tply_mesh_base));
//    DWORD old_texcount = 0, new_texcount = 0, fvf_wo_texcount = 0;
//    BOOL forced_bamp = FALSE;
//    FILE* m_fp = NULL;
//    loading_successes = false;
//    m_bonelist = NULL;
//    m_vertlist = NULL;
//    m_meshlist = NULL;
//    m_polylist = NULL;
//    m_adjalist = NULL;
//    m_shdwlist = NULL;
//    memset(m_bbox, 0, sizeof(v3_t) * 2);
//    memset(m_basis, 0, sizeof(v3_t) * 4);
//    num_weights = 0, num_tex_coords = poly_count = m_bones = m_numverts = m_numpolys = m_numadjas = m_numshdws = m_mirror = unknown_data_size = 0;
//    subskin_count = IndexType = 0;
//    specular_rgba_color = GlFvf = GlFlags = 0;
//    Bply = false;
//    memset(subskin_bones, 0, MAX_PATH);
//    has_pos = FALSE, has_rhw = FALSE, has_weights = FALSE, has_matrix_indices = FALSE, has_normal = FALSE, has_psize = FALSE, has_w = FALSE;
//    has_diffuse = FALSE, has_specular = FALSE, has_tex_coords = FALSE, has_mesh_specular = FALSE, has_mesh_bump = FALSE;
//    BAS2 = SHDW = ADJA = MROR = BNDS = SKINNED = FALSE;
//    FixPathDelim(plyname);
//    m_fp = fopen(plyname, "rb");
//    if (!m_fp) { return; }
//    m_meshlist = new CMeshList();
//    fread(sig, 1, 4, m_fp);
//    if (strncmp((char*)sig, "BPLY", 4) && strncmp((char*)sig, "EPLY", 4)) {
//# ifdef ALTERNATIVE_LANG
//        MessageBoxA(AfxGetApp()->m_pMainWnd->m_hWnd, CString("Invalid mesh file:\n" + CString(plyname)), "ERROR: CPly::CPly", MB_ICONHAND);
//#else
//        MessageBoxA(AfxGetApp()->m_pMainWnd->m_hWnd, CString("Недействительный мэш-файл:\n" + CString(plyname)), "ERROR: CPly::CPly", MB_ICONHAND);
//#endif
//        fclose(m_fp);
//        m_fp = NULL;
//        return;
//    }
//    if (sig[0] == 'B') {
//        tex_count = 2;
//        Bply = true;
//    }
//    while (fread(sig, 1, 4, m_fp) != NULL) {
//        sig[4] = '\0';
//        skip = false;
//        for (sigindx = 0; ply_siglist[sigindx] && !skip; sigindx++) {
//            if (!stricmp((char*)sig, ply_siglist[sigindx])) {
//                switch (sigindx) {
//                    case 0: //BNDS
//                    {
//                            BNDS = TRUE;
//                            skip = true;
//                            fread(m_bbox, sizeof(v3_t), 2, m_fp);
//                        }
//                        break;
//                    case 9: //SUBM
//                    case 1: //MESH
//                    {
//                            skip = true;
//                            if (sigindx != 9) {
//                                fread(&ply_mesh.fvf, sizeof(int), 1, m_fp);
//                                fread(&ply_mesh.first_face, sizeof(int), 1, m_fp);
//                                fread(&ply_mesh.face_count, sizeof(int), 1, m_fp);
//                                fread(&ply_mesh.flags, sizeof(int), 1, m_fp);
//                            } else {
//                                DWORD unknown_const[14], unknown_4byte, SUBM_Flags;
//                                fread(unknown_const, 14, 1, m_fp);
//                                ply_mesh.fvf = 0x00000152;
//                                GlFvf = ply_mesh.fvf;
//                                fread(&ply_mesh.first_face, sizeof(DWORD), 1, m_fp);
//                                fread(&ply_mesh.face_count, sizeof(DWORD), 1, m_fp);
//                                ply_mesh.flags = 0x00001004;
//                                GlFlags = ply_mesh.flags;
//                                fread(&unknown_4byte, sizeof(DWORD), 1, m_fp);
//                                fread(&unknown_4byte, sizeof(DWORD), 1, m_fp);
//                                fread(&SUBM_Flags, sizeof(DWORD), 1, m_fp);
//                                tex_count = 1;
//                            }
//                            /*
//                            Если в любом MESH встретился флаг BUMP, то во всех следующих MESH предполагается флаг BUMP
//                            Абсолютно все вертексы в этом случае будут с BUMP.
//                            */

//                            memset(text_msg_buff, 0, 2048);
//                            //printf("------------ Mesh Flags -----------");
//                            if (ply_mesh.flags & MESH_FLAG_TWO_SIDED) { strcat(text_msg_buff, "MESH_FLAG_TWO_SIDED\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_USE_ALPHA) { strcat(text_msg_buff, "MESH_FLAG_USE_ALPHA\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_LIGHT) { strcat(text_msg_buff, "MESH_FLAG_LIGHT\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_PLCR) { strcat(text_msg_buff, "MESH_FLAG_PLCR\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_SKINNED) { strcat(text_msg_buff, "MESH_FLAG_SKINNED\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_SHADOW) { strcat(text_msg_buff, "MESH_FLAG_SHADOW\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_MIRRORED) { strcat(text_msg_buff, "MESH_FLAG_MIRRORED\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_BLENDTEX) { strcat(text_msg_buff, "MESH_FLAG_BLENDTEX\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_BUMP) { strcat(text_msg_buff, "MESH_FLAG_BUMP\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_SPECULAR) { strcat(text_msg_buff, "MESH_FLAG_SPECULAR\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_MATERIAL) { strcat(text_msg_buff, "MESH_FLAG_MATERIAL\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_SUBSKIN) { strcat(text_msg_buff, "MESH_FLAG_SUBSKIN\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_TWOTEX) { strcat(text_msg_buff, "MESH_FLAG_TWOTEX\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_USINGVD) { strcat(text_msg_buff, "MESH_FLAG_USINGVD\n"); }
//                            if (ply_mesh.flags & MESH_FLAG_LIGHTMAP) { strcat(text_msg_buff, "MESH_FLAG_LIGHTMAP\n"); }
//                            //printf("-----------------------------------");
//                            printf("%s", text_msg_buff);
//                            ///MessageBox(AfxGetApp() -> m_pMainWnd -> m_hWnd, text_msg_buff, "PLY", MB_OK); 

//                            //спасибо GhostNT
//                            if (GlFvf) {
//                                fvf_wo_texcount = (GlFvf | ply_mesh.fvf) & (0xffff ^ D3DFVF_TEXCOUNT_MASK);
//                                old_texcount = GlFvf & D3DFVF_TEXCOUNT_MASK;
//                                new_texcount = ply_mesh.fvf & D3DFVF_TEXCOUNT_MASK;
//                                if (old_texcount > new_texcount) { GlFvf = fvf_wo_texcount | old_texcount; } else {
//                                    GlFvf = fvf_wo_texcount | new_texcount;
//                                    GlFlags = GlFlags | ply_mesh.flags;
//                                }
//                            } else { GlFvf = ply_mesh.fvf; GlFlags = ply_mesh.flags; }
//                            if (ply_mesh.flags & MESH_FLAG_SPECULAR) { fread(&specular_rgba_color, sizeof(int), 1, m_fp); }
//                            if (ply_mesh.flags & MESH_FLAG_BUMP) { forced_bamp = TRUE; }
//                            if (ply_mesh.flags & MESH_FLAG_SKINNED) { SKINNED = TRUE; }
//                            memset(texname[0], 0, _MAX_PATH);
//                            memset(texname[1], 0, _MAX_PATH);
//                            memset(texname[2], 0, _MAX_PATH);
//                            memset(texname[3], 0, _MAX_PATH);
//                            for (i = 0; i < tex_count; i++) {
//                                BYTE texlong = 0;
//                                fread(&texlong, sizeof(BYTE), 1, m_fp);
//                                if (texlong) { fread(textemp, 1, texlong, m_fp); } else { fread(textemp, 1, 1, m_fp); break; }
//                                textemp[texlong] = '\0';
//                                if (!i) { strcpy(texname[2], textemp); } else { strcpy(texname[3], textemp); }
//                                FixPathDelim(textemp);
//                                CMainFrame* pWnd = (CMainFrame*)AfxGetMainWnd();
//                                bool tex_test = false, repeat_for_mdl = false;
//                                do {
//                                    int skip_item = -1;
//                                    char independent_path[_MAX_PATH] = { 0 };
//                                    bool find_mask = false, path_type = false;// true - абсолютный путь, false - локальный путь
//                                    for (int path_search = 6; path_search > ((!path_type) ? 0 : -1) && !tex_test; path_search--) {
//                                        if ((!find_mask && path_type) && path_search < 6) { break; }
//                                        tex_test = false;
//                                        if (textemp[0] == '/') {
//                                            // Cut off the path before "entity" and append this to it
//                                            if (textemp[1] == 'e' || textemp[1] == 't' || textemp[1] == 'E' || textemp[1] == 'T') //entity or texture
//                                            {
//                                                strcpy(texname[i], pWnd->m_ResPath[(path_search == 6) ? 0 : path_search]);
//                                                strcat(texname[i], "resource");
//                                                strcat(texname[i], textemp);
//                                                path_type = false;
//                                            }
//                                        } else {
//                                            if (path_search == 6) {
//                                                path_type = true;
//                                                if (repeat_for_mdl) {
//                                                    repeat_for_mdl = false;
//                                                    path = strrchr((char*)pWnd->m_CurMdlPath, '/');
//                                                    path++;
//                                                    strncpy((char*)texname[i], (char*)pWnd->m_CurMdlPath, (path - pWnd->m_CurMdlPath));
//                                                    texname[i][path - pWnd->m_CurMdlPath] = '\0';
//                                                    //текущий путь mdl`а
//                                                } else {
//                                                    // Cut off the path before the ply name and append this to it
//                                                    if (strlen(pWnd->m_CurMdlPath)) { repeat_for_mdl = true; }
//                                                    path = strrchr((char*)plyname, '/');
//                                                    path++;
//                                                    strncpy((char*)texname[i], (char*)plyname, (path - plyname));
//                                                    texname[i][path - plyname] = '\0';
//                                                    //путь ply-файла
//                                                }
//                                            } else {
//                                                if (skip_item >= 0 && path_search == skip_item) { continue; }
//                                                if (!strlen(pWnd->m_ResPath[path_search])) { continue; }
//                                                memset(texname[i], 0, _MAX_PATH);
//                                                sprintf(texname[i], "%s%s", pWnd->m_ResPath[path_search], independent_path);
//                                            }
//                                            //D:\Games\Men of War Assault Squad 2\mods\LTP\resource\entity\test\2\ba27m_2\
//                                            //берём путь "по умолчанию" (для path_search = 6)
//                                            if (path_search == 6) {
//                                                for (int path_mask = 5; path_mask > -1; path_mask--) {
//                                                    int ResPath_len = strlen(pWnd->m_ResPath[path_mask]);
//                                                    //D:\Games\Men of War Assault Squad 2\mods\LTP\
//                                                    //проверяем список
//                                                    char tmp_res_path[_MAX_PATH] = { 0 };
//                                                    strcpy(tmp_res_path, pWnd->m_ResPath[path_mask]);
//                                                    FixPathDelim(tmp_res_path);
//                                                    strlwr(tmp_res_path);
//                                                    strlwr(texname[i]);
//                                                    if (ResPath_len && strstr(texname[i], tmp_res_path)) {
//                                                        find_mask = true;
//                                                        strncpy(independent_path, texname[i] + ResPath_len, strlen(texname[i]) - ResPath_len);
//                                                        skip_item = path_mask;
//                                                        //resource\entity\test\2\ba27m_2\
//                                                        //получаем корень пути
//                                                        break;
//                                                    }
//                                                }
//                                            }
//                                            strcat((char*)texname[i], (char*)textemp);// часть пути (если есть в ply), имя и расширение файла материала
//                                        }
//                                        FixPathDelim(texname[i]);
//                                        All_Trash* AT = new All_Trash();
//                                        AT->Parse_Path(texname[i]);
//                                        delete AT;
//                                        //тут проверка пути
//                                        //проверяем на tex`овость или полное имя
//                                        FILE* testfp = NULL;
//                                        char ExtTest[MAX_PATH] = { 0 };
//                                        strcpy(ExtTest, texname[i]);
//                                        if (!(testfp = fopen(ExtTest, "r"))) {
//                                            strcat(ExtTest, ".tex");
//                                            if (testfp = fopen(ExtTest, "r")) {
//                                                strcat(texname[i], ".tex");
//                                                tex_test = true;
//                                                repeat_for_mdl = false;
//                                            }
//                                        } else {
//                                            tex_test = true;
//                                            repeat_for_mdl = false;
//                                        }
//                                        if (testfp) { fclose(testfp); testfp = NULL; }
//                                        //ищем расширения
//                                        if (!tex_test) {
//                                            for (int texsigindx = 0; tex_siglist[texsigindx]; texsigindx++) {
//                                                strcpy(ExtTest, texname[i]);
//                                                strcat(ExtTest, tex_siglist[texsigindx]);
//                                                if (testfp = fopen(ExtTest, "r")) {
//                                                    strcat(texname[i], tex_siglist[texsigindx]);
//                                                    tex_test = true;
//                                                    fclose(testfp);
//                                                    testfp = NULL;
//                                                    repeat_for_mdl = false;
//                                                    break;
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                                while (repeat_for_mdl);
//                                if (!tex_test) {
//# ifdef ALTERNATIVE_LANG
//                                    MessageBoxA(AfxGetApp()->m_pMainWnd->m_hWnd, CString("The material file was not found:\n" + CString(textemp)), "ERROR: CPly::CPly", MB_ICONHAND);
//#else
//                                    MessageBoxA(AfxGetApp()->m_pMainWnd->m_hWnd, CString("Файл материала не найден:\n" + CString(textemp)), "ERROR: CPly::CPly", MB_ICONHAND);
//#endif
//                                    memset(texname[i], 0, _MAX_PATH);
//                                    strcpy(texname[i], "<<checker>>");
//                                }
//                            }
//                            if (ply_mesh.flags & MESH_FLAG_SUBSKIN) {
//                                fread(&subskin_count, sizeof(BYTE), 1, m_fp);
//                                for (i = 0; i < subskin_count; i++) { fread(&subskin_bones[i], sizeof(BYTE), 1, m_fp); }
//                            }
//                            if (strlen(texname[0])) { m_CurrMesh = m_meshlist->AddToTail(ply_mesh.first_face, ply_mesh.face_count, ply_mesh.fvf, texname, tex_count, ply_mesh.flags, specular_rgba_color, subskin_count, subskin_bones); }
//                        }
//                        break;
//                    case 2: //VERT
//                    {
//                            skip = true;
//                            fread(&m_numverts, sizeof(int), 1, m_fp);
//                            fread(&m_vsize, sizeof(short), 1, m_fp);
//                            fread(&m_vflags, sizeof(short), 1, m_fp);
//                            unknown_data_size = m_vsize;
//                            if (GlFvf & D3DFVF_POSITION_MASK) {
//                                has_pos = TRUE;
//                                unknown_data_size -= 12;
//                                if ((GlFvf & D3DFVF_XYZB5) >= D3DFVF_XYZB1) {
//                                    num_weights = ((GlFvf & D3DFVF_XYZB5) - D3DFVF_XYZB1) >> 1;
//                                    has_weights = TRUE;
//                                    unknown_data_size -= 4 * (num_weights + 1);
//                                    if (GlFvf & D3DFVF_LASTBETA_UBYTE4) { has_matrix_indices = TRUE; } else
//                                    if (GlFvf & D3DFVF_LASTBETA_D3DCOLOR) { has_matrix_indices = TRUE; } else { num_weights += 1; }
//                                } else
//                                if ((GlFvf & D3DFVF_POSITION_MASK) == D3DFVF_XYZW) {
//                                    has_w = TRUE;
//                                    unknown_data_size -= 4;
//                                } else
//                                if ((GlFvf & D3DFVF_POSITION_MASK) == D3DFVF_XYZRHW) {
//                                    has_rhw = TRUE;
//                                    unknown_data_size -= 4;
//                                }
//                            }
//                            if (GlFvf & D3DFVF_NORMAL) {
//                                has_normal = TRUE;
//                                unknown_data_size -= 12;
//                            }
//                            if (GlFvf & D3DFVF_PSIZE) {
//                                has_psize = TRUE;
//                                unknown_data_size -= 4;
//                            }
//                            if (GlFvf & D3DFVF_DIFFUSE) {
//                                has_diffuse = TRUE;
//                                unknown_data_size -= 4;
//                            }
//                            if (GlFvf & D3DFVF_SPECULAR) {
//                                has_specular = TRUE;
//                                unknown_data_size -= 4;
//                            }
//                            if (GlFvf & D3DFVF_TEXCOUNT_MASK) {
//                                num_tex_coords = (GlFvf & D3DFVF_TEXCOUNT_MASK) >> D3DFVF_TEXCOUNT_SHIFT;
//                                if (GlFlags & MESH_FLAG_TWOTEX) { num_tex_coords *= 2; }
//                                if (Bply && m_vsize == 36)//дичь
//                                    { num_tex_coords -= 1; CopyUV = true; }
//                                unknown_data_size -= 8 * num_tex_coords;
//                                has_tex_coords = TRUE;
//                            }
//                            has_mesh_specular = ((GlFvf & MESH_FLAG_SPECULAR) != 0);
//                            has_mesh_bump = ((GlFvf & MESH_FLAG_BUMP) != 0) | forced_bamp;
//                            if (has_mesh_bump) {
//                                if (unknown_data_size < 16) { has_mesh_bump = FALSE; } else { unknown_data_size -= 16; }
//                            }
//                            m_calculated_vsize = 0;
//                            m_calculated_vsize = (has_pos) ? sizeof(v3_t) : 0;
//                            m_calculated_vsize += (has_rhw) ? 4 : 0;
//                            m_calculated_vsize += (has_weights && num_weights > 0) ? (num_weights * 4) : 0;
//                            m_calculated_vsize += (has_matrix_indices) ? 4 : 0;
//                            m_calculated_vsize += (has_normal) ? sizeof(v3_t) : 0;
//                            m_calculated_vsize += (has_psize) ? 4 : 0;
//                            m_calculated_vsize += (has_diffuse) ? sizeof(int) : 0;
//                            m_calculated_vsize += (has_specular) ? 4 : 0;
//                            m_calculated_vsize += (has_tex_coords && num_tex_coords > 0) ? (num_tex_coords * sizeof(v2_t)) : 0;
//                            m_calculated_vsize += (has_mesh_bump) ? 16 : 0;
//                            memset(text_msg_buff, 0, 2048);
//                            sprintf(text_msg_buff, "Vertex size: %d\nСalculated vsize: %d\nVertex count: %d\nVertex flags: %X\n------------ D3D9 FVF -----------\nFVF flags: %X\nHas position: %s\nHas RHW: %s\nHas weights: %s\nNum weights: %d\n"
    
//                                "Has matrix indices: %s\nHas normal: %s\nHas psize: %s\nHas diffuse: %s\nHas specular: %s\nHas tex coords: %s\nNum tex coords: %d\nHas bump: %s", m_vsize, m_calculated_vsize, m_numverts, m_vflags, ply_mesh.fvf, (has_pos) ? "TRUE" : "FALSE",
//                                (has_rhw) ? "TRUE" : "FALSE", (has_weights) ? "TRUE" : "FALSE", num_weights, (has_matrix_indices) ? "TRUE" : "FALSE", (has_normal) ? "TRUE" : "FALSE", (has_psize) ? "TRUE" : "FALSE",
//                                (has_diffuse) ? "TRUE" : "FALSE", (has_specular) ? "TRUE" : "FALSE", (has_tex_coords) ? "TRUE" : "FALSE", num_tex_coords, (has_mesh_bump) ? "TRUE" : "FALSE");
//                            ///MessageBox(AfxGetApp() -> m_pMainWnd -> m_hWnd, text_msg_buff, "PLY", MB_OK);

//                            m_vertlist = new vert_t[m_numverts];
//                            memset(m_vertlist, 0, sizeof(vert_t) * m_numverts);
//                            for (i = 0; i < m_numverts; i++) {
//                                if (has_pos) {
//                                    fread(&m_vertlist[i].xyz, sizeof(v3_t), 1, m_fp);
//                                    if (has_rhw || has_w) { fread(&m_vertlist[i].rhw, 4, 1, m_fp); }
//                                    if (num_weights > 0) {
//                                        m_vertlist[i].WeightsData = new DWORD[num_weights];
//                                        for (i2 = 0; i2 < num_weights; i2++) { fread(&m_vertlist[i].WeightsData[i2], 4, 1, m_fp); }
//                                    }
//                                    if (has_matrix_indices) {
//                                        fread(&m_vertlist[i].bones[0], 1, 1, m_fp);
//                                        fread(&m_vertlist[i].bones[1], 1, 1, m_fp);
//                                        fread(&m_vertlist[i].bones[2], 1, 1, m_fp);
//                                        fread(&m_vertlist[i].bones[3], 1, 1, m_fp);
//                                    }
//                                }
//                                if (has_normal) { fread(&m_vertlist[i].vn, sizeof(v3_t), 1, m_fp); }
//                                if (has_psize) { fread(&m_vertlist[i].psize, 4, 1, m_fp); }
//                                if (has_diffuse) {
//                                    fread(&m_vertlist[i].real_diffuse, 4, 1, m_fp);
//                                    m_vertlist[i].diffuse = m_vertlist[i].real_diffuse;
//                                    if (GetAValue(m_vertlist[i].diffuse) == 0) { m_vertlist[i].diffuse |= 0xFF000000; }
//                                } else { m_vertlist[i].diffuse = 0xFFFFFFFF; }
//                                //m_vertlist[i].diffuse = 0x80FFFFFF;
//                                if (has_specular) { fread(&m_vertlist[i].specular, 4, 1, m_fp); }
//                                if (num_tex_coords > 0) {
//                                    fread(&m_vertlist[i].uv[0], sizeof(v2_t), 1, m_fp);
//                                    if (num_tex_coords == 2) { fread(&m_vertlist[i].uv[1], sizeof(v2_t), 1, m_fp); }
//                                    if (Bply && CopyUV) { memcpy(m_vertlist[i].uv[1], m_vertlist[i].uv[0], sizeof(v2_t)); }
//                                    float AddU = 0, AddV = 0;
//                                    for (i2 = 0; i2 < (num_tex_coords - 2); i2++) {
//                                        fread(&AddU, 4, 1, m_fp);
//                                        fread(&AddV, 4, 1, m_fp);
//                                    }
//                                }
//                                if (has_mesh_bump) {
//                                    fread(&m_vertlist[i].bump12, sizeof(v3_t), 1, m_fp);
//                                    fread(&m_vertlist[i].bump4, sizeof(float), 1, m_fp);
//                                    //memcpy(&m_vertlist[i].vn, &m_vertlist[i].bump12, sizeof(v3_t));
//                                }
//                                if (unknown_data_size > 0) { fread(&m_vertlist[i].other_buff, unknown_data_size, 1, m_fp); }
//                            }
//                        }
//                        break;
//                    case 3: //INDX
//                    {
//                            skip = true;
//                            IndexType = 1;
//                            fread(&INDXcount, sizeof(int), 1, m_fp);
//                            m_numpolys = INDXcount / 3;
//                            poly_count = INDXcount / 3;
//                            m_polylist = new indx_t[m_numpolys];
//                            memset(m_polylist, 0, sizeof(indx_t) * m_numpolys);
//                            for (i = 0; i < m_numpolys; i++) {
//                                WORD tshort[3];
//                                fread(tshort, sizeof(WORD), 3, m_fp);
//                                /*if(SKINNED)
//                                {
//                                    m_polylist[i].v[0] = tshort[2];//
//                                    m_polylist[i].v[1] = tshort[1];// Этот костыль чисто для этой программы
//                                    m_polylist[i].v[2] = tshort[0];//
//                                }
//                                else*/
//                                //{
//                                m_polylist[i].v[0] = tshort[0];
//                                m_polylist[i].v[1] = tshort[1];
//                                m_polylist[i].v[2] = tshort[2];
//                                //}
//                                //fread(&m_polylist[i], sizeof(indx_t), 1, m_fp);
//                            }
//                        }
//                        break;
//                    case 7: //IND4
//                    {
//                            skip = true;
//                            IndexType = 2;
//                            fread(&INDXcount, sizeof(int), 1, m_fp);
//                            poly_count = INDXcount / 3;
//                            m_numpolys = INDXcount / 3;
//                            m_polylist = new indx_t[m_numpolys];
//                            memset(m_polylist, 0, sizeof(indx_t) * m_numpolys);
//                            for (i = 0; i < INDXcount / 3; i++) { fread(&m_polylist[i], sizeof(indx_t), 1, m_fp); }
//                        }
//                        break;
//                    case 4: //ADJA
//                    {
//                            ADJA = TRUE;
//                            skip = true;
//                            fread(&m_numadjas, sizeof(int), 1, m_fp);
//                            m_adjalist = new adja_t[m_numadjas];
//                            for (i = 0; i < m_numadjas; i++) { fread(&m_adjalist[i], sizeof(adja_t), 1, m_fp); }
//                        }
//                        break;
//                    case 5: //SHDW
//                    {
//                            SHDW = TRUE;
//                            skip = true;
//                            fread(&m_numshdws, sizeof(int), 1, m_fp);
//                            m_shdwlist = new BYTE[m_numshdws];
//                            for (i = 0; i < m_numshdws; i++) { fread(&m_shdwlist[i], 1, 1, m_fp); }
//                        }
//                        break;
//                    case 6: //SKIN
//                    {
//                            skip = true;
//                            BYTE csize = 0;
//                            fread(&m_bones, sizeof(int), 1, m_fp);
//                            if (m_bones) {
//                                m_bonelist = new char*[m_bones];
//                                for (i = 0; i < m_bones; i++) {
//                                    fread(&csize, 1, sizeof(char), m_fp);
//                                    m_bonelist[i] = new char[csize + 1];
//                                    fread(m_bonelist[i], 1, csize, m_fp);
//                                    m_bonelist[i][csize] = '\0';
//                                }
//                            }
//                        }
//                        break;
//                    case 8: //MROR
//                    {
//                            MROR = TRUE;
//                            skip = true;
//                            m_mirror = 1;
//                        }
//                        break;
//                    case 10: //BAS2
//                    {
//                            BAS2 = TRUE;
//                            skip = true;
//                            fread(m_basis, sizeof(v3_t) * 4, 1, m_fp);
//                        }
//                        break;
//                    default: //Other
//                    {
//# ifdef ALTERNATIVE_LANG
//                            MessageBoxA(AfxGetApp()->m_pMainWnd->m_hWnd, "A valid but unreadable section of the file was found.", "ERROR: CPly::CPly", MB_ICONHAND);
//#else
//                            MessageBoxA(AfxGetApp()->m_pMainWnd->m_hWnd, "Найден допустимый, но нечитаемый раздел файла.", "ERROR: CPly::CPly", MB_ICONHAND);
//#endif
//                            fclose(m_fp);
//                            return;
//                        }
//                        break;
//                }
//            }
//        }
//        if (!ply_siglist[sigindx - 1]) {
//            CStringA st;
//            long failure_address = ftell(m_fp);
//# ifdef ALTERNATIVE_LANG
//            st.Format("File: %s\r\ndamaged or has an unknown format.\r\nStudy the problem in the hexadecimal editor in the address area: 0x%x", plyname, failure_address);
//#else
//            st.Format("Файл: %s\r\nповреждён или имеет неизвестный формат.\r\nИзучай проблему в шестнадцатеричном редакторе в области адреса: 0x%x", plyname, failure_address);
//#endif
//            MessageBoxA(AfxGetApp()->m_pMainWnd->m_hWnd, st, "ERROR: CPly::CPly", MB_ICONHAND);
//            fclose(m_fp);
//            return;
//        }
//    }
//    fclose(m_fp);
//    loading_successes = true;
//    return;
//}