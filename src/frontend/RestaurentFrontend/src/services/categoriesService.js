import axiosInstance from "../axios/axiosInstance"

export class CategoryService{
    async GetCategories(){
        try {
            let response = await axiosInstance.get("/api/Categories")
            return response.data

        } catch (error) {
            logger.log("CategoryService :: GetCategories :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }

    async GetCategoryById(categoryId){
        try {
            let response = await axiosInstance.get(`/api/Categories/${categoryId}`)
            return response.data
        
        } catch (error) {
            logger.log("CategoryService :: GetCategoryById :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }

    async AddCategory({ categoryName, status,cat_Image}) {
        try {
            const formData = new FormData();
            formData.append('CategoryName', categoryName);
            formData.append('Status', status);
            formData.append('Cat_Image', cat_Image[0]);

            const response = await axiosInstance.post(`/api/Categories/add-category`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

            return response.data;
        } catch (error) {
            logger.error("CategoryService :: AddCategory :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            throw error;
        }
    }

    async GetAllCategoriesForAdmin() {
    try {
            const response = await axiosInstance.get("/api/Categories/admin/categories");
            return response.data;
        } catch (error) {
            logger.log("CategoryService :: GetAllCategoriesForAdmin :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }

    async UpdateCategoryStatus(status,categoryId){
        try {
            let response = await axiosInstance.put(`/api/Categories`,{
                CategoryId:categoryId,
                Status:status
            })
            return response.data;
        } catch (error) {
            logger.error("CategoryService :: UpdateCategoryStatus :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }
}

const categoryService = new CategoryService()
export default categoryService