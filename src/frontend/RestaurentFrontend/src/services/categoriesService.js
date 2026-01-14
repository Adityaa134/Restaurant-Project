import axiosInstance from "../axios/axiosInstance"

export class CategoryService{
    async GetCategories(){
        try {
            let response = await axiosInstance.get("/Categories")
            return response.data

        } catch (error) {
            console.log("CategorService :: GetCategories :: ",error)
            return false
        }
    }

    async GetategoryById(categoryId){
        try {
            let response = await axiosInstance.get(`/Categories/${categoryId}`)
            return response.data
        
        } catch (error) {
            console.log("CategorService :: GetCategoryById :: ",error)
            return false
        }
    }

    async AddCategory({ categoryName, status,cat_Image}) {
        try {
            const formData = new FormData();
            formData.append('CategoryName', categoryName);
            formData.append('Status', status);
            formData.append('Cat_Image', cat_Image[0]);

            const response = await axiosInstance.post(`/Categories/add-category`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

            return response.data;
        } catch (error) {
            console.log("CategoryService :: AddCategory :: ", error);
            throw error;
        }
    }

    async GetAllCategoriesForAdmin() {
    try {
            const response = await axiosInstance.get("/Categories/admin/categories");
            return response.data;
        } catch (error) {
            console.log("CategorService :: GetAllCategoriesForAdmin :: ",error)
            return false
        }
    }

    async UpdateCategoryStatus(status,categoryId){
        try {
            let response = await axiosInstance.put(`/Categories`,{
                CategoryId:categoryId,
                Status:status
            })
            return response.data;
        } catch (error) {
            console.log("CategoryService :: UpdateCategoryStatus :: ", error)
        }
    }
}

const categoryService = new CategoryService()
export default categoryService